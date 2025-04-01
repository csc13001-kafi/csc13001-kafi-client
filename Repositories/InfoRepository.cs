using System.Net.Http;
using System.Threading.Tasks;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories
{
    public interface IInfoRepository
    {
        Task UpdateInfo(UserRequest request);
        Task UpdateProfileImage(ImageRequest request);
    }

    public class InfoRepository(IInfoDao dao) : IInfoRepository
    {
        private readonly IInfoDao _dao = dao;
        public async Task UpdateInfo(UserRequest request)
        {
            await _dao.UpdateInfo(request);
        }

        public async Task UpdateProfileImage(ImageRequest request)
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(request.FileStream);
            fileContent.Headers.Add("Content-Type", request.ContentType);

            form.Add(fileContent, "image", request.FileName);

            await _dao.UpdateProfileImage(form);
        }
    }
}
