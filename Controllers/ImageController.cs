using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;

namespace ImageGeneratorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        // POST api/image/generate
        // This endpoint receives image parameters and returns a generated image.
        [HttpPost("generate")]
        public IActionResult GenerateImage([FromBody] ImageParameters parameters)
        {
            try
            {
                // validate the received parameters
                if (!IsValidParameters(parameters))
                {
                    return BadRequest("Invalid parameters.");
                }

                // generate the image
                using var imageStream = GenerateCustomImage(parameters);

                return File(imageStream.ToArray(), $"image/{parameters.Format}");
            }
            catch (Exception ex)
            {
                // handle any exceptions that occur during processing
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsValidParameters(ImageParameters parameters)
        {
            return parameters.Width > 0 && parameters.Height > 0;
        }

        private MemoryStream GenerateCustomImage(ImageParameters parameters)
        {
            // create SKBitmap and customize the image using SkiaSharp
            using var bitmap = new SKBitmap(parameters.Width, parameters.Height);
            using var canvas = new SKCanvas(bitmap);

            // customize the image based on parameters (colors, shapes, text, etc.)
            // example: Fill the image with a solid color
            using var paint = new SKPaint
            {
                Color = SKColor.Parse(parameters.BackgroundColor)
            };
            canvas.DrawRect(0, 0, parameters.Width, parameters.Height, paint);

            // Convert SKBitmap to byte array
            using var image = SKImage.FromBitmap(bitmap);
            var imageStream = new MemoryStream();
            image.Encode(SKEncodedImageFormatFromParameter(parameters.Format), 100)
                 .SaveTo(imageStream);
            imageStream.Seek(0, SeekOrigin.Begin);

            return imageStream;
        }

        private SKEncodedImageFormat SKEncodedImageFormatFromParameter(string format)
        {
            return format.ToLower() switch
            {
                "jpeg" or "jpg" => SKEncodedImageFormat.Jpeg,
                "png" => SKEncodedImageFormat.Png,
                "gif" => SKEncodedImageFormat.Gif,
                _ => SKEncodedImageFormat.Png,
            };
        }
    }

    public class ImageParameters
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public required string Format { get; set; }
        public required string BackgroundColor { get; set; }
    }
}