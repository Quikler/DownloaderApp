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
    internal class InstagramModel : InputOutputModel
    {
        private static readonly string AccountSessionFilePath = $"{AppContext.BaseDirectory}account_session.dat";

        public InstaService InstagramService { get; }
        public IInstaApi Api { get; }
        public Regex Regex { get; }
        public InstaMediaInfos? Infos { get; set; }

        public InstagramModel()
        {
            Regex = new Regex(
                @"(?:https?://)(?:www\.)instagram\.com/(?:p|reel|reels)/([a-zA-Z0-9_-]{1,11})(\/\?\S*)?\/?$",
                RegexOptions.IgnoreCase);

            Api = InstaApiBuilder
                .CreateBuilder()
                .SetSessionHandler(new FileSessionHandler { FilePath = AccountSessionFilePath })
                .Build();

            Api.SessionHandler.Load();

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
