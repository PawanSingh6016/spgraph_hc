/*
This is not used currently
*/
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using TypeSystem;

namespace Subscriptions
{
    // public class SPFolderSubscription : ObjectType{



    //     protected override void Configure(IObjectTypeDescriptor descriptor)
    //     {
    //         // descriptor.Field("FolderAdded")
    //         // .Type<GraphTypes.GraphSpFolder>()
    //         // .Resolve(context => context.GetEventMessage<SPFolder>())
    //         // .Subscribe(async context =>
    //         // {
    //         //     var receiver = context.Service<ITopicEventReceiver>();
    //         //     ISourceStream stream = await receiver.SubscribeAsync<string, SPFolder>("FolderAdded");
    //         //     return stream;
    //         // });
    //         //  descriptor.Field(t => t.OnReview(default, default))
    //         // .Type<NonNullType<ReviewType>>()
    //         // .Argument("episode", arg => arg.Type<NonNullType<EpisodeType>>());
    //     }

    // }

    public class SPFolderSubscription
    {

        [SubscribeAndResolve]
        public ValueTask<ISourceStream<SPFolder>> FolderAdded(string webUrl,
    [Service] ITopicEventReceiver receiver)
        {
            var topic = $"FolderCreated_{webUrl}";

            return receiver.SubscribeAsync<string, SPFolder>(topic);
        }
    }

}