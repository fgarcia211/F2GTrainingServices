namespace F2GTraining.Helpers
{
    public class HelperRutasProvider
    {
        private IWebHostEnvironment hostEnvironment;

        public HelperRutasProvider(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public string MapPath(string fileName)
        {
            string carpeta = "images/equipos";
            string rootPath = this.hostEnvironment.WebRootPath;
            string path = Path.Combine(rootPath, carpeta, fileName);

            if (System.IO.File.Exists(path))
            {
                string[] partName = fileName.Split("-");

                return this.generateNewRoute(partName[0].Split(".")[0], carpeta, rootPath);
            }
            else
            {
                return path;
            }
            
        }

        public string generateNewRoute(string name, string carpet, string rootPath)
        {
            bool contained = true;
            string path = "";

            while (contained == true)
            {
                int random = new Random().Next(1, 1000);
                string fileName = name + "-" + random + ".png";

                path = Path.Combine(rootPath, carpet, fileName);

                if (!(System.IO.File.Exists(path)))
                {
                    contained = false;
                }
            }

            return path;
        }
        
    }
}
