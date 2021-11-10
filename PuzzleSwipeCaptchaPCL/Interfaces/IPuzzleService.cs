using System;
using System.IO;
using System.Threading.Tasks;
using PuzzleSwipeCaptchaPCL.Models;

namespace PuzzleSwipeCaptchaPCL.Interfaces
{
    public interface IPuzzleService
    {
        Puzzle CreateLocalPuzzleAsync(Stream imageStream);
        Task<Puzzle> CreateRemotePuzzleAsync(string imageUrl);

        bool IsPuzzleSolved(PuzzleInfo submission, PuzzleInfo record);
    }
}