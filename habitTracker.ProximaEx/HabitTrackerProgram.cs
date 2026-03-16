using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using PExFormatting;

namespace habitTracker.ProximaEx;

class HabitTrackerProgram
{
	const string connectionString = "Data Source=habit-log.db";
	static readonly SqliteConnection connection = new(connectionString);
	static List<HabitType> habitTypes = [];
	static int currentHabitIndex = 0;
	static bool SelectAllAdded = false;
	static string viewAllQueryCondition = " ";
	static readonly bool seedData = true;
	static readonly int seedNum = 100;
	static Random rand = new();

	static void Main(string[] args)
	{
		FormatLibrary.SetWindow(72,25);
		CreateTablesIfNotExists();
		GetHabitTypes();
		FlashTitle();
		CoffeeAnimation();
		Menu();
		Console.Clear();
	}

	static void FlashTitle()
	{
		string[] titleScreen =
		{
			"                 ___   ___               ",
			"   ____   ____  ||``\\ ||``\\  ___   ___   ",
			"  ||``\\| ||``|| ||__  ||__  ||`|| ||`||  ",
			"  ||     ||  || |!``  |!``  |!==' |!=='  ",
			"  ||__/| ||__|| ||    ||    ||__. ||__.  ",
			"   ````   ````  ``    ``     ````  ````  ",
			"",
			"",
			"  .                        ",
			" /  __.  __~  __~  __.  ,_ ",
			"/  /__/ /__/ /__/ /--` /  `",
			"        ,_/  ,_/  `-       "
		};
		int topY = Console.WindowHeight / 2 - titleScreen.Length / 2;
		Console.SetCursorPosition(0, topY);
		foreach (string line in titleScreen) { Console.WriteLine(FormatLibrary.CenterText(line)); }
		Thread.Sleep(1200);

		string[,] notJustCoffee = { { FormatLibrary.CenterText("=========================================") } };
		FormatLibrary.Animate(notJustCoffee, [0, topY + 3], 500);
		string[,] message = { { FormatLibrary.CenterText("h a b i t") } };
		FormatLibrary.Animate(message, [0, topY + 6], 1500);

		Console.Clear();
	}

	static void CoffeeAnimation()
	{	
		string[,] loadCells =
		{
			{
			"        \\ .         ",
			"       ' ) )        ",
			"        / / '       ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
			{
			"       ' ) ;        ",
			"        / / '       ",
			"       (  \\         ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
			{
			"        ./ `        ",
			"       ( (.         ",
			"       ,\\  \\        ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
			{
			"       c.(          ",
			"        ;\\ \\        ",
			"       ' ) .)       ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
		};
		int[] topLeftCoordsForCenteredAnimation =
			[ Console.WindowWidth / 2 - loadCells[0,0].Length / 2 , Console.WindowHeight / 2 - loadCells.GetLength(1) / 2 ];
		for (int i = 0; i < 3; i++) { FormatLibrary.Animate(loadCells, topLeftCoordsForCenteredAnimation, 250); }
	}

	static void Menu()
	{	
		string[] menuStrings = [
				"\n",
				"Welcome to Coffee, um I mean Habit Logger",
				"Track whatever...",
				"\n",
				"   +----------------+   ",
				"<  |                |  >",
				"   +----------------+   ",
				"\n",
				"Please select an option:",
				"+--------------------------------+",
				"|    1 - Log habit occurance     |",
				"|    2 - View habit history      |",
				"|    3 - Edit a record           |",
				"|    4 - Delete a record         |",
				"|    5 - Add a new habit type    |",
				"+--------------------------------+",
				"\n\n\n",
				"Type 'exit' at any time to go back"
		];
		bool exit = false;
		while (!exit)
		{
			FormatLibrary.Draw(menuStrings);
			if (!SelectAllAdded && habitTypes.Count >= 2)
			{ 
				habitTypes.Add(new HabitType("All"));
				HabitType.Types.Add("All");
				SelectAllAdded = true; 
			}
			Console.SetCursorPosition(Console.WindowWidth / 2 - 7, 7);
			Console.Write(FormatLibrary.CenterText(habitTypes[currentHabitIndex].Type, 14));
			Console.SetCursorPosition(Console.WindowWidth / 2 - 1, 20);

			var keyInput = Console.ReadKey(false);
			if (keyInput.Key == ConsoleKey.LeftArrow || keyInput.Key == ConsoleKey.RightArrow)
			{
				HabitSelector(keyInput);
			}
			else
			{
				string menuInput = char.ToString(keyInput.KeyChar) + Console.ReadLine();
				viewAllQueryCondition = 
					habitTypes[currentHabitIndex].Type == "All" ? " " : $"WHERE Habit = '{habitTypes[currentHabitIndex].Type}'";

				switch (menuInput)
				{
					case "exit":
						exit = true;
						break;
					case "1":
						Insert();
						break;
					case "2":
						ViewAll(viewAllQueryCondition);
						FormatLibrary.SectionExitMessage("");
						break;
					case "3":
						Edit();
						break;
					case "4":
						Delete();
						break;
					case "5":
						AddType();
						break;
					default:
						Console.Write(FormatLibrary.CenterText("Please enter a valid option."));
						Thread.Sleep(500);
						break;
				}
			}
		}
		Thread.Sleep(1000);
	}

	static void Insert()
	{
		Console.Clear();
		string? type = habitTypes[currentHabitIndex].Type;
		string? unit = habitTypes[currentHabitIndex].Unit;
		if (habitTypes[currentHabitIndex].Type == "All")
		{
			type = GetHabitInput();
			if (type == "exitOption") { return; }
			unit = habitTypes[HabitType.Types.IndexOf(type)].Unit;
		}
		string date = GetDateInput();
		if (date == "exitOption") { return; }
		int quantity = GetNumInput($"Enter quantity of {unit}");
		if (quantity == -1) { return; }

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				$@"INSERT INTO habit_table ( Date, Quantity, Habit ) VALUES ( '{date}', {quantity}, '{type}')";
			tableCmd.ExecuteNonQuery();
			connection.Close();
		}
		FormatLibrary.SectionExitMessage("Record added!  ");
	}

	static void ViewAll(string queryCondition = " ")
	{
		Console.Clear();
		List<HabitRecord> queryRecords = new();

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText = 
				@$"SELECT * FROM habit_table {queryCondition}";
			var reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					queryRecords.Add( new HabitRecord(reader.GetInt32(0),reader.GetString(1),reader.GetInt32(2),reader.GetString(3)));
				}
			} else { Console.WriteLine(FormatLibrary.CenterText("No data found\n")); }

			reader.Close();
			connection.Close();
		}
		if (queryRecords.Count > 0)
		{
			Console.WriteLine(
				"\n " + 
				" Rec".PadLeft(Console.WindowWidth/2 - 2, '-') + 
				"ords ".PadRight(Console.WindowWidth/2, '-') +
				"\n"
				);
			foreach (HabitRecord row in queryRecords)
			{
				Console.WriteLine(FormatLibrary.CenterText(
					$"Id: {row.Id}".PadRight(9) + 
					$"{row.Date}" + 
					$"{row.Habit}".PadLeft(20) +
					$"{ habitTypes[HabitType.Types.IndexOf(row.Habit)].Unit }: {row.Quantity}".PadLeft(20)
					));
			}
			Console.WriteLine("\n" + " ".PadRight(Console.WindowWidth - 1, '-') + "\n");
		}
	}

	static void ViewTypes(string? queryCondition = " ")
	{
		Console.Clear();
		string[] rows = [];

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				@$"SELECT * FROM habit_types {queryCondition}";
			var reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					Array.Resize(ref rows, rows.Length + 1);
					rows[rows.Length - 1] = reader.GetString(0);
				}
			}
			else { Console.WriteLine(FormatLibrary.CenterText("No data found\n")); }

			reader.Close();
			connection.Close();
		}
		if (rows.Length > 0)
		{
			int p = 1;
			Console.WriteLine("\n" + FormatLibrary.CenterText("----------- Habit Types -----------\n"));
			foreach (string row in rows)
			{
				Console.WriteLine(FormatLibrary.CenterText($"{p++.ToString().PadLeft(3)}. {row.PadLeft(14)}  "));
			}
			Console.WriteLine("\n" + FormatLibrary.CenterText("-----------------------------------\n"));
		}
	}

	static void Edit()
	{
		while (true)
		{
			Console.Clear();
			ViewAll(viewAllQueryCondition);
			int id = GetNumInput("Enter Id for record you want to change");
			if (id == -1) { return; }

			using (connection)
			{
				connection.Open();
				var scalarCmd = connection.CreateCommand();
				scalarCmd.CommandText =
					$@"SELECT EXISTS (SELECT 1 FROM habit_table WHERE Id = {id})";
				int isMatch = Convert.ToInt32(scalarCmd.ExecuteScalar());

				if (isMatch == 0)
				{
					connection.Close();
					Console.WriteLine(FormatLibrary.CenterText("No match was found"));
					Thread.Sleep(500);
				}
				else
				{
					string date = GetDateInput();
					if (date == "exitOption") { return; }
					int quantity = GetNumInput("Enter new quantity");
					if (quantity == -1) { return; }

					var tableCmd = connection.CreateCommand();
					tableCmd.CommandText =
						@$"UPDATE habit_table SET Date = '{date}', Quantity = {quantity} WHERE Id = {id}";
					tableCmd.ExecuteNonQuery();
					connection.Close();

					FormatLibrary.SectionExitMessage("Edit Successful.  ");
					return;
				}
			}
		}
	}

	static void Delete()
	{
		Console.Clear();
		ViewAll(viewAllQueryCondition);
		int id = GetNumInput("Enter the Id of the record you want to delete");
		if (id == -1) { return; }
		string searchParam = $"WHERE Id = {id}";
		if (viewAllQueryCondition != " ") { searchParam = $"{viewAllQueryCondition} AND Id = {id}"; }	

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText = 
				$@"DELETE FROM habit_table {searchParam}";
			int deletedRows = tableCmd.ExecuteNonQuery();
			if (deletedRows == 0) { FormatLibrary.SectionExitMessage("Couldn't find that record.  "); }
			else { FormatLibrary.SectionExitMessage("Record deleted.  "); }
			connection.Close();
		}
	}

	static int GetNumInput(string message)
	{
		while (true)
		{
			Console.WriteLine("\n" + FormatLibrary.CenterText(message));
			FormatLibrary.CenterCursor();
			string? input = Console.ReadLine();
			if (input == "exit") { return -1; }
			bool parsed = int.TryParse(input, out int result);

			if (parsed && result > 0 && result < 5000) { return result; }
			else if (!parsed) { Console.WriteLine(FormatLibrary.CenterText("Invalid input, please enter a number")); }
			else if (result <= 0 || result >= 5000) { Console.WriteLine(FormatLibrary.CenterText("Invalid input, out of range 0-5000")); }
			else { Console.WriteLine(FormatLibrary.CenterText("Invalid input")); }
		}
	}

	static string GetDateInput()
	{
		while (true)
		{
			Console.WriteLine("\n" + FormatLibrary.CenterText("Enter date (formatted mm-dd-yy) or press Enter for today"));
			FormatLibrary.CenterCursor(8);
			string? input = Console.ReadLine();
			if (input == "exit") { return "exitOption"; }
			if (input == "") { return DateTime.Now.ToString("MM-dd-yy"); }
			if (DateTime.TryParseExact(input, "MM-dd-yy", new CultureInfo("en-US"), 0, out DateTime outyDate)) { return input; }
			else { Console.WriteLine(FormatLibrary.CenterText("Invalid date")); }
		}
	}

	static string GetHabitInput()
	{
		while (true)
		{
			ViewTypes();
			Console.WriteLine("\n" + FormatLibrary.CenterText("Enter habit type"));
			FormatLibrary.CenterCursor(8);
			string? input = Console.ReadLine();
			if ( input == "exit" ) { return "exitOption"; }
			if (input != null && HabitType.Types.Contains(input) ) { return input; }
			else { Console.WriteLine(FormatLibrary.CenterText("Habit not found, exit to main menu to create a new type")); }
		}
	}

	static void HabitSelector(ConsoleKeyInfo readKey)
	{
		if (readKey.Key == ConsoleKey.RightArrow)
		{ currentHabitIndex = ((currentHabitIndex + habitTypes.Count + 1) % habitTypes.Count); }
		if (readKey.Key == ConsoleKey.LeftArrow)
		{ currentHabitIndex = ((currentHabitIndex + habitTypes.Count - 1) % habitTypes.Count); }
	}

	static void GetHabitTypes()
	{
		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				@$"SELECT * FROM habit_types";
			var reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					habitTypes.Add( new HabitType(reader.GetString(0),reader.GetString(1)) );
					HabitType.Types.Add(reader.GetString(0));
				}
			}
			reader.Close();
			connection.Close();
		}
	}

	static void CreateTablesIfNotExists()
	{
		string[] initialCmds =
		[
			@"CREATE TABLE IF NOT EXISTS habit_table (
				Id INTEGER PRIMARY KEY AUTOINCREMENT,
				Date TEXT,
				Quantity INTEGER,
				Habit TEXT
				)",
			@"CREATE TABLE IF NOT EXISTS habit_types (
				Type TEXT PRIMARY KEY,
				Unit TEXT
				)",
			@"REPLACE INTO habit_types ( Type , Unit ) VALUES ( 'Coffee' , 'Cups' )"
		];
		int isEmpty = 0;
		using (connection)
		{
			connection.Open();
			foreach (string cmd in initialCmds)
			{
				var tableCmd = connection.CreateCommand();
				tableCmd.CommandText = cmd;
				tableCmd.ExecuteNonQuery();
			}
			var scalarCmd = connection.CreateCommand();
			scalarCmd.CommandText =
				$@"SELECT EXISTS (SELECT 1 FROM habit_table)";
			isEmpty = Convert.ToInt32(scalarCmd.ExecuteScalar());
			connection.Close();
		}
		if (isEmpty == 0 && seedData) { SeedData(); }
	}

	static void AddType()
	{
		Console.Clear();
		ViewTypes();
		string newHabit = NewTypeInfo("Enter a new habit type");
		if (newHabit == "exitOption") { return; }
		string newUnit = NewTypeInfo("What unit should this habit type track?", 40);
		if (newUnit == "exitOption") { return; }

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				$@"REPLACE INTO habit_types ( Type , Unit ) VALUES ('{newHabit}' , '{newUnit}') ";
			tableCmd.ExecuteNonQuery();
			connection.Close();
		}
		if (!HabitType.Types.Contains(newHabit))
		{
			habitTypes.Add(new HabitType(newHabit, newUnit));
			HabitType.Types.Add(newHabit);
			FormatLibrary.SectionExitMessage("Habit type added!  ");
		}
		else { FormatLibrary.SectionExitMessage("Habit type already exists.  "); }
	}

	public static string NewTypeInfo(string question, int charLimit = 14)
	{
		while (true)
		{
			var startPosition = Console.GetCursorPosition();
			Console.WriteLine("\n" + FormatLibrary.CenterText(question));
			FormatLibrary.CenterCursor(8);
			string? newValue = Console.ReadLine();
			if (newValue == "exit") { return "exitOption"; }
			else if (newValue == null || newValue.Length > charLimit || newValue.Length < 1)
			{
				Console.WriteLine(FormatLibrary.CenterText($"Invalid length, must be < {charLimit} characters"));
				Thread.Sleep(500);
				Console.SetCursorPosition(startPosition.Left,startPosition.Top);
				for (int i = 0; i < 3; i++) { Console.WriteLine("".PadLeft(Console.WindowWidth)); }
				Console.SetCursorPosition(startPosition.Left, startPosition.Top);
			}
			else { return newValue; }
		}
	}

	static void SeedData()
	{
		string[,] seedHabits =
		{
			{ "Coffee" , "Drink Water" , "Code" , "Small Talk" , "Protien" , "Paper Cuts" },
			{ "Cups" , "Cups" , "Hours" , "Interactions" , "Grams" , "Pain" }
		};
		using (connection)
		{
			connection.Open();

			for (int i = 0; i < seedHabits.GetLength(1); i++)
			{
				var seedHabsCmd = connection.CreateCommand();
				string seedHab = seedHabits[0,i];
				string seedUnit = seedHabits[1, i];
				seedHabsCmd.CommandText =
				$@"REPLACE INTO habit_types ( Type , Unit ) VALUES ( '{seedHab}' , '{seedUnit}' )";
				seedHabsCmd.ExecuteNonQuery();
			}
			for (int i = 0; i < seedNum; i++)
			{
				var tableCmd = connection.CreateCommand();
				DateOnly startDate = new(2026, 3, 15);
				startDate = startDate.AddDays(i-seedNum);
				string date = startDate.ToString("MM-dd-yy");
				string habit = seedHabits[0, rand.Next(0, 6)];
				int quantity = rand.Next(1,10);
				tableCmd.CommandText =
				$@"INSERT INTO habit_table ( Date, Quantity, Habit ) VALUES ( '{date}', {quantity}, '{habit}')";
				tableCmd.ExecuteNonQuery();
			}
			connection.Close();
		}
	}
}

public class HabitRecord(int idIn, string dateIn, int quantityIn, string habit)
{
	public int Id { get; } = idIn;
	public string Date { get; } = dateIn;
	public int Quantity { get; } = quantityIn;
	public string Habit { get; } = habit;
}

public class HabitType(string typeIn, string unitIn = "Units")
{
	public static List<string> Types { get; set; } = [];
	public string Type { get; set; } = typeIn;
	public string Unit { get; set; } = unitIn;
}
