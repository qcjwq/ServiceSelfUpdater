using Nancy;

namespace ServerUpdateWebHost.Modules
{
    public class VersionModule : NancyModule
    {
        public VersionModule()
        {
            Get["/nancy/Home"] = p =>
            {
                return View["/Home"];
            };
        }
    }
}