using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Metro.Domain.Services;
using Metro.Shared.Models;

namespace Metro.Infrastructure.File.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;
        private readonly string _requestPath;

        public FileStorageService(IConfiguration configuration, IHostEnvironment environment)
        {
            var configuredRoot = configuration["Storage:RootPath"] ?? Path.Combine("Public", "Storage");
            _rootPath = Path.IsPathRooted(configuredRoot)
                ? configuredRoot
                : Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredRoot));

            _requestPath = (configuration["Storage:RequestPath"] ?? "/Storage").TrimEnd('/');

            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveAsync(Base64FileModel file, string? folder = null, CancellationToken cancellationToken = default)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(file.ContentBase64))
            {
                throw new ArgumentException("Conteúdo do arquivo é obrigatório.", nameof(file));
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                throw new ArgumentException("Nome do arquivo é obrigatório.", nameof(file));
            }

            var safeFolder = SanitizeFolder(folder);
            var targetDirectory = string.IsNullOrEmpty(safeFolder)
                ? _rootPath
                : Path.Combine(_rootPath, safeFolder);

            Directory.CreateDirectory(targetDirectory);

            var safeFileName = SanitizeFileName(file.FileName);
            var storedFileName = $"{Guid.NewGuid():N}-{safeFileName}";
            var absolutePath = Path.Combine(targetDirectory, storedFileName);

            var bytes = Convert.FromBase64String(NormalizeBase64(file.ContentBase64));
            await System.IO.File.WriteAllBytesAsync(absolutePath, bytes, cancellationToken);

            return string.IsNullOrEmpty(safeFolder)
                ? storedFileName
                : Path.Combine(safeFolder, storedFileName).Replace('\\', '/');
        }

        public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return Task.CompletedTask;
            }

            var absolutePath = ResolveAbsolutePath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }

            return Task.CompletedTask;
        }

        public bool Exists(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return false;
            }

            return System.IO.File.Exists(ResolveAbsolutePath(relativePath));
        }

        public string GetPublicPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }

            var normalized = relativePath.Replace('\\', '/').TrimStart('/');
            return $"{_requestPath}/{normalized}";
        }

        private string ResolveAbsolutePath(string relativePath)
        {
            var normalized = relativePath.Replace('\\', '/').TrimStart('/');
            var absolutePath = Path.GetFullPath(Path.Combine(_rootPath, normalized));

            if (!absolutePath.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Caminho de arquivo inválido.");
            }

            return absolutePath;
        }

        private static string SanitizeFolder(string? folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return string.Empty;
            }

            var cleaned = folder
                .Replace('\\', '/')
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(SanitizeSegment)
                .Where(segment => !string.IsNullOrWhiteSpace(segment));

            return string.Join('/', cleaned);
        }

        private static string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName).Trim();
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '_');
            }

            return string.IsNullOrWhiteSpace(name) ? "file" : name;
        }

        private static string SanitizeSegment(string segment)
        {
            if (segment is "." or "..")
            {
                return string.Empty;
            }

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                segment = segment.Replace(invalidChar, '_');
            }

            return segment;
        }

        private static string NormalizeBase64(string contentBase64)
        {
            var value = contentBase64.Trim();
            var commaIndex = value.IndexOf(',');
            if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            {
                value = value[(commaIndex + 1)..];
            }

            return Regex.Replace(value, @"\s+", string.Empty);
        }
    }
}
