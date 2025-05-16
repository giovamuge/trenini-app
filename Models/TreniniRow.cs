namespace TreniniApp.Models;

public record TrainRow(
    string Time,
    string Train,
    string Destination,
    string? Track,
    string? Delay,
    string? Category,
    string? Vect,
    int Index = 0,
    Position Position = Position.Middle
);

public enum Position
{
    First,
    Middle,
    Last
}