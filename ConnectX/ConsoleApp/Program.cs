
using MenuSystem;

Console.WriteLine("Hello, World!");

var menu0 = new Menu("TIC-TAC-TOE Main Menu", EMenuLevel.Root);
var menu1 = new Menu("TIC-TAC-TOE Level1 Menu", EMenuLevel.First);
var menu2 = new Menu("TIC-TAC-TOE Level2 Menu", EMenuLevel.Deep);
var menu3 = new Menu("TIC-TAC-TOE Level3 Menu", EMenuLevel.Deep);



menu0.addMenuItem("a", "Level0 - Option A, returns x", () =>
{
    Console.WriteLine("Level0 - Option A was called");
    return "x";
});

menu1.addMenuItem("n", "Level1 - Option A, returns b", () =>
{
    Console.WriteLine("Level1 - Option A was called");
    return "b";
});

menu2.addMenuItem("n", "Level2 - Option A, returns m", () =>
{
    Console.WriteLine("Level2 - Option A was called");
    return "m";
});

menu3.addMenuItem("n", "Level3 - Option A, return z", () =>
{
    Console.WriteLine("Level3 - Option A was called");
    return "z";
});



menu0.addMenuItem("1", "Level0 - Go To Level 1", menu1.Run);
menu1.addMenuItem("2", "Level1 - Go To Level 2", menu2.Run);
menu2.addMenuItem("3", "Level2 - Go To Level 3", menu3.Run);

menu0.Run();

Console.WriteLine("We are DONE...");
