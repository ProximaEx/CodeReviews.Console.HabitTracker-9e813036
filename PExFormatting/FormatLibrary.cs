using System;

namespace PExFormatting;

public static class FormatLibrary
{
	public static void SetWindow(int targetW, int targetH)
	{
		int width = (targetW > Console.LargestWindowWidth) ? Console.LargestWindowWidth : targetW;
		int height = (targetH > Console.LargestWindowHeight) ? Console.LargestWindowHeight : targetH;
		if (OperatingSystem.IsWindows())
		{
			int defaultBuffH = Console.BufferHeight;
			Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, defaultBuffH);
		}
	}

	public static void Draw(string[] cell)
	{
		Console.Clear();
		foreach (string s in cell) { Console.WriteLine(CenterText(s)); }
	}

	public static string CenterText(string input)
	{
		return input.PadLeft(Console.WindowWidth / 2 + input.Length / 2);
	}

	public static string CenterText(string input, int boundWidth)
	{
		return input.PadLeft(boundWidth / 2 + input.Length / 2);
	}

	public static void CenterCursor(int boundBoxWidth = 4)
	{
		var current = Console.GetCursorPosition();
		Console.SetCursorPosition(Console.WindowWidth / 2 - boundBoxWidth / 2, current.Top);
	}

	public static void ClearRange(int[] topLeftCoords, int[] bottomRightCoords)
	{
		int rowQuantity = bottomRightCoords[1] - topLeftCoords[1] + 1;
		int clearLength = bottomRightCoords[0] - topLeftCoords[0] + 1;
		for (int i = 0; i < rowQuantity; i++)
		{
			Console.SetCursorPosition(topLeftCoords[0], topLeftCoords[1] + i);
			Console.Write("".PadLeft(clearLength));
		}
	}

	public static void Animate(string[,] cells, int[] topLeftCoords, int threadSleep)
	{
		int[] bottomRightCoords = { topLeftCoords[0] + cells[0, 0].Length - 1, topLeftCoords[1] + cells.GetLength(1) - 1 };
		for (int i = 0; i < cells.GetLength(0); i++)
		{
			ClearRange(topLeftCoords, bottomRightCoords);
			for (int j = 0; j < cells.GetLength(1); j++)
			{
				Console.SetCursorPosition(topLeftCoords[0], topLeftCoords[1] + j);
				Console.Write(cells[i, j]);
			}
			Thread.Sleep(threadSleep);
		}
	}

	public static void SectionExitMessage(string exitMessage)
	{
		Console.WriteLine("\n" + CenterText($"{exitMessage}Press any key to exit"));
		CenterCursor(2);
		Console.ReadKey(true);
	}
}