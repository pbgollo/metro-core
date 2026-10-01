using Metro.Shared.Models;

namespace Metro.Domain.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Salva o arquivo no disco e retorna o caminho relativo para persistir no banco
        /// (ex: "avatars/3f2a...-foto.png").
        /// </summary>
        Task<string> SaveAsync(Base64FileModel file, string? folder = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);

        bool Exists(string relativePath);

        /// <summary>
        /// Caminho público para servir o arquivo (ex: "/Storage/avatars/3f2a...-foto.png").
        /// </summary>
        string GetPublicPath(string relativePath);
    }
}
