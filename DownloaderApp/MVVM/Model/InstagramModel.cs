using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramService.Classes;
using InstagramService.Classes.Collections;
using InstagramService.Classes.Models;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace DownloaderApp.MVVM.Model
{
    internal class InstagramModel
    {
        public static string AccountSessionFilePath { get; } = $"{AppContext.BaseDirectory}account_session.dat";
        public static Regex Regex { get; } = new Regex(
                @"(?:https?://)(?:www\.)instagram\.com/(?:p|reel|reels)/([a-zA-Z0-9_-]{1,11})(\/\?\S*)?\/?$",
                RegexOptions.IgnoreCase);

        public InstaService InstagramService { get; set; }
        public IInstaApi Api { get; set; }
        public InstaMediaInfos? Infos { get; set; }

        public InstagramModel()
        {
            Api = InstaApiBuilder
                .CreateBuilder()
                .SetSessionHandler(new FileSessionHandler { FilePath = AccountSessionFilePath })
                .Build();

            Api.SessionHandler.Load(false);

            InstagramService = new InstaService(Api);
        }

        public async Task<IResult<InstaMediaStreams>> GetStreamsFromAsync(IEnumerable<MediaElement> selectableCollection)
        {
            // grabing all medias that has been selected
            IEnumerable<InstaMediaInfo> selectedMedias = Infos!
                .Where(info => selectableCollection.Any(m => m.Source.AbsoluteUri == info.Uri));

            int selectedSize = selectedMedias.Count();
            InstaMediaInfos selectedMediaInfos = new(selectedSize, Infos!.Media);
            for (int i = 0; i < selectedSize; i++)
            {
                InstaMediaInfo current = selectedMedias.ElementAt(i);
                selectedMediaInfos[i] = new(current.MediaType, current.Uri, current.InitialUri, current.CarouselIndex);
            }

            return await InstagramService.StreamTaker.GetMediaStreamsAsync(selectedMediaInfos);
        }
    }
}
