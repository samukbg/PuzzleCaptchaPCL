using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleCaptchaPCL.Models
{
    public class PuzzleInfo
    {
        public string Id { get; set; }

        public int X { get; set; }

        public int NumberOfTries { get; set; }

        public DateTimeOffset SubmittedAt { get; set; }

        public DateTimeOffset ExpiredAt { get; set; }
    }
}