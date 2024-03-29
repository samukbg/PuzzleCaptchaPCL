﻿using Azure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleCaptchaPCL.Models
{
    public class PuzzleEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
    }
}