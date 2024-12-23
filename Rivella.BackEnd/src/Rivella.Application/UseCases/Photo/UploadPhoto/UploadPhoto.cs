using Rivella.Application.Common;
using Rivella.Application.Interfaces;
using Rivella.Domain.Repository;

namespace Rivella.Application.UseCases.Photo.UploadPhoto;

public class UploadPhoto(
    IAlbumRepository albumRepository,
    IPhotoRepository photoRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork) : IUploadPhoto
{
    public async Task Upload(UploadPhotoInput input)
    {
        var album = await albumRepository.GetAsync(input.IdAlbum) ??
            throw new NullReferenceException($"Album com Id {input.IdAlbum} não encontrado");

        var filename = StorageFileName.Create(album.Id);
        var photoUrl = await storageService.Upload(filename, input.Image);
        var photo = new Domain.Entity.Photo(
            album.Id,
            photoUrl,
            input.UserName,
            input.Description
        );
        
        await photoRepository.InsertAsync(photo);
        await unitOfWork.CommitAsync();
    }
}