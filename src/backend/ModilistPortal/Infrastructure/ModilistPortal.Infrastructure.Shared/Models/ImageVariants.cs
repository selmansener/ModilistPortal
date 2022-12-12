using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Models
{
    public class ImageVariants : ValueObject<ImageVariants>
    {
        public ImageVariants(
            string? thumbnail = null,
            string? xSmall = null,
            string? small = null,
            string? medium = null,
            string? large = null,
            string? xLarge = null)
        {
            Thumbnail = thumbnail;
            XSmall = xSmall;
            Small = small;
            Medium = medium;
            Large = large;
            XLarge = xLarge;
        }

        public string? Thumbnail { get; private set; }

        public string? XSmall { get; private set; }

        public string? Small { get; private set; }

        public string? Medium { get; private set; }

        public string? Large { get; private set; }

        public string? XLarge { get; private set; }

        public ImageVariants SetVariants(
            string? thumbnail = null,
            string? xSmall = null,
            string? small = null,
            string? medium = null,
            string? large = null,
            string? xLarge = null)
        {
            return new ImageVariants(
                thumbnail ?? Thumbnail,
                xSmall ?? XSmall,
                small ?? Small,
                medium ?? Medium,
                large ?? Large,
                xLarge ?? XLarge);
        }
    }
}
