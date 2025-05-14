namespace TreniniApp.Models;

public record TrainRow(
    string Time,
    string Train,
    string Destination,
    string? Track,
    string? Delay,
    string? Category,
    string? Vect
);
