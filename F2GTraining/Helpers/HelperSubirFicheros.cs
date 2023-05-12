namespace F2GTraining.Helpers
{
    public class HelperSubirFicheros
    {
        private HelperRutasProvider helperRuta;
        public HelperSubirFicheros(HelperRutasProvider pathProvider)
        {
            this.helperRuta = pathProvider;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string nombre)
        {
            string fileName = nombre.Replace(" ", string.Empty) + ".png";
            string path = this.helperRuta.MapPath(fileName);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
