using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using PuzzleCaptchaPCL.Interfaces;
using PuzzleCaptchaPCL.Models;
using SkiaSharp;

namespace PuzzleCaptchaPCL.Services
{
    public class PuzzleService : IPuzzleService
    {
        const int PIECE_WIDTH = 135;
        const int PIECE_HEIGHT = 120;
        const int TAB_RADIUS = 20;


        public bool IsPuzzleSolved(PuzzleInfo submission, PuzzleInfo record)
        {
            if (submission.Id != record.Id) return false;
            if (Math.Abs(submission.X - record.X) > 30) return false;
            if (submission.SubmittedAt > record.ExpiredAt) return false;

            return true;
        }

        private int[,] GetMissingPieceData()
        {
            int[,] data = new int[PIECE_WIDTH, PIECE_HEIGHT];

            double c1 = (PIECE_WIDTH - TAB_RADIUS) / 2;
            double c2 = PIECE_HEIGHT / 2;
            double squareOfTabRadius = Math.Pow(TAB_RADIUS, 2);

            double xBegin = PIECE_WIDTH - TAB_RADIUS;
            double yBegin = TAB_RADIUS;

            for (int i = 0; i < PIECE_WIDTH; i++)
            {
                for (int j = 0; j < PIECE_HEIGHT; j++)
                {
                    double d1 = Math.Pow(i - c1, 2) + Math.Pow(j, 2);
                    double d2 = Math.Pow(i - xBegin, 2) + Math.Pow(j - c2, 2);
                    if ((j <= yBegin && d1 < squareOfTabRadius) || (i >= xBegin && d2 > squareOfTabRadius))
                    {
                        data[i, j] = 0;
                    }
                    else
                    {
                        data[i, j] = 1;
                    }
                }
            }

            return data;
        }

        private int[,] GetMissingPieceBorderData(int[,] d)
        {
            int[,] borderData = new int[PIECE_WIDTH, PIECE_HEIGHT];

            for (int i = 0; i < d.GetLength(0); i++)
            {
                for (int j = 0; j < d.GetLength(1); j++)
                {
                    if (d[i, j] == 0) continue;

                    if (i - 1 < 0 || j - 1 < 0 || i + 1 >= PIECE_WIDTH || j + 1 >= PIECE_HEIGHT)
                    {
                        borderData[i, j] = 1;

                        continue;
                    }

                    int sumOfSourrounding =
                        d[i - 1, j - 1] + d[i, j - 1] + d[i + 1, j - 1] +
                        d[i - 1, j] + d[i + 1, j] +
                        d[i - 1, j + 1] + d[i, j + 1] + d[i + 1, j + 1];

                    if (sumOfSourrounding != 8)
                    {
                        borderData[i, j] = 1;
                    }
                }
            }

            return borderData;
        }

        private (SKBitmap MissingPiece, SKBitmap Puzzle) GenerateMissingPieceAndPuzzle(SKBitmap originalImage, int x, int y)
        {
            SKBitmap missingPiece = new SKBitmap(PIECE_WIDTH, PIECE_HEIGHT);

            int[,] missingPiecePattern = GetMissingPieceData();
            int[,] missingPieceBorderPattern = GetMissingPieceBorderData(missingPiecePattern);

            for (int i = 0; i < PIECE_WIDTH; i++)
            {
                for (int j = 0; j < PIECE_HEIGHT; j++)
                {
                    int templatePattern = missingPiecePattern[i, j];
                    SKColor originalArgb = originalImage.GetPixel(x + i, y + j);

                    if (templatePattern == 1)
                    {
                        bool isBorder = missingPieceBorderPattern[i, j] == 1;
                        missingPiece.SetPixel(i, j, isBorder ? SKColors.White : originalArgb);

                        originalImage.SetPixel(x + i, y + j, FilterPixel(originalImage, x + i, y + j));
                    }
                    else
                    {
                        missingPiece.SetPixel(i, j, SKColors.Transparent);
                    }
                }
            }

            return (missingPiece, originalImage);
        }

        private SKColor FilterPixel(SKBitmap img, int x, int y)
        {
            const int KERNEL_SIZE = 3;
            int[,] kernel = new int[KERNEL_SIZE, KERNEL_SIZE];

            int xStart = x - 1;
            int yStart = y - 1;
            for (int i = xStart; i < KERNEL_SIZE + xStart; i++)
            {
                for (int j = yStart; j < KERNEL_SIZE + yStart; j++)
                {
                    int tx = i;
                    if (tx < 0)
                    {
                        tx = -tx;
                    }
                    else if (tx >= img.Width)
                    {
                        tx = x - 1;
                    }

                    int ty = j;
                    if (ty < 0)
                    {
                        ty = -ty;
                    }
                    else if (ty >= img.Height)
                    {
                        ty = y - 1;
                    }

                    kernel[i - xStart, j - yStart] = ((int)(uint)img.GetPixel(tx, ty));

                }
            }

            int r = 0;
            int g = 0;
            int b = 0;
            int count = KERNEL_SIZE * KERNEL_SIZE;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    Color c = (i == j) ? Color.Black : Color.FromArgb(kernel[i, j]);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                }
            }

            return new SKColor((byte)(r / count), (byte)(g / count), (byte)(b / count));
        }

        private string GetImageBase64(SKBitmap image)
        {
            var stream = new MemoryStream();
            SKData encodedData = image.Encode(SKEncodedImageFormat.Png, 100);
            encodedData.SaveTo(stream);

            encodedData.AsStream();

            byte[] imageBytes = encodedData.ToArray();

            string base64ImageContent = Convert.ToBase64String(imageBytes);

            return base64ImageContent;
        }

        public Puzzle CreateLocalPuzzleAsync(Stream imageStream)
        {
            var jigsawPuzzle = new Puzzle();

            try
            {
                if (imageStream == null)
                {
                    Console.Write("ImageStream is null");
                }
                else
                {
                    SKBitmap originalImage = SKBitmap.Decode(imageStream);
                    Random random = new Random();

                    int xRandom = random.Next(originalImage.Width - 2 * PIECE_WIDTH) + PIECE_WIDTH;
                    int yRandom = random.Next(originalImage.Height - PIECE_HEIGHT);

                    var puzzle = GenerateMissingPieceAndPuzzle(originalImage, xRandom, yRandom);

                    jigsawPuzzle.BackgroundImage = puzzle.Puzzle;
                    jigsawPuzzle.MissingPieceImage = puzzle.MissingPiece;
                    jigsawPuzzle.X = xRandom;
                    jigsawPuzzle.Y = yRandom;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return jigsawPuzzle;
        }

        public async Task<Puzzle> CreateRemotePuzzleAsync(string imageUrl)
        {
            var jigsawPuzzle = new Puzzle();
            var uri = new Uri(imageUrl);

            try
            {
                using (WebClient wc = new WebClient())
                {
                    Stream s = await wc.OpenReadTaskAsync(uri);
                    
                    SKBitmap originalImage = SKBitmap.Decode(s);
                    Random random = new Random();


                    int xRandom = random.Next(originalImage.Width - 2 * PIECE_WIDTH) + PIECE_WIDTH;
                    int yRandom = random.Next(originalImage.Height - PIECE_HEIGHT);

                    var puzzle = GenerateMissingPieceAndPuzzle(originalImage, xRandom, yRandom);

                    jigsawPuzzle.BackgroundImage = puzzle.Puzzle;
                    jigsawPuzzle.MissingPieceImage = puzzle.MissingPiece;
                    jigsawPuzzle.X = xRandom;
                    jigsawPuzzle.Y = yRandom;
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return jigsawPuzzle;
        }
    }
}