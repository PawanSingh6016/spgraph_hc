using System.Threading.Tasks;
using DataServices;

namespace Mutations {

    public class SPMutationFolder{

        protected Microsoft.AspNetCore.Http.IHttpContextAccessor accessor;
        protected DataServices.Services services;

        public SPMutationFolder(Microsoft.AspNetCore.Http.IHttpContextAccessor ctx)
        {
            this.accessor = ctx;
            services = new DataServices.Services(ctx);
        }
        
        public async Task<TypeSystem.SPFolder> AddFolder(string weburl, string parent,string folderName){
            var result = await services.sp_createFolder(weburl,parent,folderName);            
            return result;
            //return 1;
        }
    }
}