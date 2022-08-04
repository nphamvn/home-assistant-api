using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HomeAssistant.API.Models.OptionModels;
using Microsoft.Extensions.Options;

namespace HomeAssistant.API.Services;

public class PhotoStorageService
{
    private readonly CloudinaryOptions options;

    public PhotoStorageService(IOptions<CloudinaryOptions> options)
    {
        this.options = options.Value;
    }
    public async Task<string> UploadPhoto(string name, Stream stream)
    {
        Cloudinary cloudinary = new(options.Url);

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(name, stream)
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        return uploadResult.Url.OriginalString;
    }
}