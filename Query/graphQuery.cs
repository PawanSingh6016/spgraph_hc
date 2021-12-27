using System.Collections.Generic;
using HotChocolate.Types;
using TypeSystem;

namespace GraphQuery
{
    public class Query
    {

        protected Microsoft.AspNetCore.Http.IHttpContextAccessor accessor;
        private DataServices.Services services;

        public Query(Microsoft.AspNetCore.Http.IHttpContextAccessor ctx)
        {
            this.accessor = ctx;
            services = new DataServices.Services(accessor);
        }

        public SPSite GetSiteCollection(string url)
        {   
            return services.sp_getSite(url).Result;
        }

        public List<SPSite> GetSubSites(string url)
        {         
            return services.sp_getSubsites(url).Result;
        }

        public List<SPFolder> GetSiteFolders(string siteurl)
        {         
            return services.sp_getSiteFolders(siteurl).Result;
        }

        public List<SPFile> GetFolderFiles(string folderUrl)
        {            
            return services.sp_getFilesInFolder(folderUrl).Result;
        }
        public List<SPFolder> GetFolders(string folderUrl)
        {
            return services.sp_getFoldersInFolder(folderUrl).Result;
        }        
    }

    public class GraphQuery : ObjectType<Query>
    {
        protected Microsoft.AspNetCore.Http.IHttpContextAccessor accessor;
        public GraphQuery(Microsoft.AspNetCore.Http.IHttpContextAccessor ctx) => this.accessor = ctx;
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field("spSite")
            .Argument("url", a => a.Type<NonNullType<StringType>>())
            .Resolve(context =>
            {
                var argUrl = context.ArgumentValue<string>("url");
                return new Query(accessor).GetSiteCollection(argUrl);
            }).Type<GraphTypes.GraphSpSite>();
        }
    }
}