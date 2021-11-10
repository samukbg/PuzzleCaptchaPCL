using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace PuzzleCaptchaPCL.Models
{
    public class Puzzle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public SKBitmap BackgroundImage { get; set; }

        public SKBitmap MissingPieceImage { get; set; }

        public string MissingPieceSource { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}