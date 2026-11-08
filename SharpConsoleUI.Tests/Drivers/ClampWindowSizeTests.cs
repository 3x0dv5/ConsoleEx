using SharpConsoleUI.Drivers.Input;
using Xunit;

namespace SharpConsoleUI.Tests.Drivers;

/// <summary>
/// Tests for <see cref="TerminalRawMode.ClampWindowSize"/>, the pure postcondition behind every
/// terminal-size read: never return a non-positive dimension, and report when a default was
/// substituted for one.
/// </summary>
public class ClampWindowSizeTests
{
	[Fact]
	public void ClampsNegativeHeightToDefault()
	{
		// The exact TERM=dumb case: terminfo reports cols but no lines.
		var (width, height, substituted) = TerminalRawMode.ClampWindowSize(80, -1);

		Assert.Equal(80, width);
		Assert.Equal(TerminalRawMode.DefaultWindowHeight, height);
		Assert.True(substituted);
	}

	[Fact]
	public void ClampsNegativeWidthAndHeight()
	{
		// The TERM=linux case: neither dimension is reported.
		var (width, height, substituted) = TerminalRawMode.ClampWindowSize(-1, -1);

		Assert.Equal(TerminalRawMode.DefaultWindowWidth, width);
		Assert.Equal(TerminalRawMode.DefaultWindowHeight, height);
		Assert.True(substituted);
	}

	[Fact]
	public void ClampsZeroToDefault()
	{
		// The TERM-unset case: 0x0 is legal at the buffer, but still a substitution at the driver.
		var (width, height, substituted) = TerminalRawMode.ClampWindowSize(0, 0);

		Assert.Equal(TerminalRawMode.DefaultWindowWidth, width);
		Assert.Equal(TerminalRawMode.DefaultWindowHeight, height);
		Assert.True(substituted);
	}

	[Fact]
	public void PreservesValidReportedSize()
	{
		// A real terminal's reported size passes through unchanged.
		var (width, height, substituted) = TerminalRawMode.ClampWindowSize(120, 40);

		Assert.Equal(120, width);
		Assert.Equal(40, height);
		Assert.False(substituted);
	}
}
