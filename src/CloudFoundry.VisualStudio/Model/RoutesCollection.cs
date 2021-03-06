﻿using CloudFoundry.CloudController.V2.Client;
using CloudFoundry.CloudController.V2.Client.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CloudFoundry.VisualStudio.Model
{
    class RoutesCollection : CloudItem
    {
        private readonly CloudFoundryClient _client;
        private readonly ListAllSpacesForOrganizationResponse _space;

        public RoutesCollection(ListAllSpacesForOrganizationResponse space, CloudFoundryClient client)
            : base(CloudItemType.RoutesCollection)
        {
            _client = client;
            _space = space;
        }

        public override string Text
        {
            get
            {
                return "Routes";
            }
        }

        protected override System.Drawing.Bitmap IconBitmap
        {
            get
            {
                return Resources.Routes;
            }
        }

        protected override async Task<IEnumerable<CloudItem>> UpdateChildren()
        {
            List<Route> result = new List<Route>();

            PagedResponseCollection<ListAllRoutesForSpaceResponse> routes = await _client.Spaces.ListAllRoutesForSpace(this._space.EntityMetadata.Guid);


            while (routes != null && routes.Properties.TotalResults != 0)
            {
                foreach (var route in routes)
                {
                    var routeApps = await _client.Routes.ListAllAppsForRoute(route.EntityMetadata.Guid);

                    var domain = await _client.DomainsDeprecated.RetrieveDomainDeprecated(route.DomainGuid);
                    result.Add(new Route(route, domain, routeApps, this._client));
                }

                routes = await routes.GetNextPage();
            }

            return result;
        }

        protected override IEnumerable<CloudItemAction> MenuActions
        {
            get
            {
                return null;
            }
        }
    }
}
