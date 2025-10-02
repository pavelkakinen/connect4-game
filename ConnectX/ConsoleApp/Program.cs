using MenuSystem;

Console.WriteLine("Hello, ConnectFour!");


var menu = new Menu("Connect Four", EMenuLevel.Root);
menu.AddMenuItem("a", "Level0 - Option A", () => { Console.WriteLine("Level0 - Option A was called");
    return "a";
});

var menu1 = new Menu("Menu 1", EMenuLevel.First);
menu1.AddMenuItem("a", "Level1 - Option A", () => { Console.WriteLine("Level1 - Option A was called");
    return "a";
});

var menu2 = new Menu("Menu 2", EMenuLevel.Deep);
menu2.AddMenuItem("a", "Level2 - Option A", () => { Console.WriteLine("Level2 - Option A  was called");
    return "a";
});

var menu3 = new Menu("Menu 3", EMenuLevel.Deep);
menu3.AddMenuItem("a", "Level3 - Option A", () => { Console.WriteLine("Level3 - Option A  was called");
    return "a";
});


menu.AddMenuItem("1", "Level0 - Go to Level1", menu1.Run);

menu1.AddMenuItem("2", "Level1 - Go to Level2", menu2.Run);

menu2.AddMenuItem("3", "Level2 - Go to Level3", menu3.Run);


menu.Run();