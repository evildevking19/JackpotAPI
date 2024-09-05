namespace ApiServer.Utils;

public class ImageLib {
    public static String storeFile(String filename, String base64DataUrl) {
        var uploadsFolder = "wwwroot/uploads";
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
        var realBase64Str = base64DataUrl.Substring(base64DataUrl.IndexOf(',')+1);
        var filepath = $"{uploadsFolder}/{filename}";
        File.WriteAllBytes(filepath, Convert.FromBase64String(realBase64Str));
        return filepath;
    }

    public static String getImageDataUrl(String filepath, String mimeType) {
        if (!File.Exists(filepath))
            return "";
        byte[] imageBytes = File.ReadAllBytes(filepath);
        string base64String = Convert.ToBase64String(imageBytes);
        return $"data:{mimeType};base64,{base64String}";
    }

    public static void removeImage(String filepath) {
        File.Delete(filepath);
    }
}