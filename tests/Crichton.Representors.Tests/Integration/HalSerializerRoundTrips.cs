using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Representors.Tests.Integration
{
    public class HalSerializerRoundTrips : TestWithFixture
    {
        private HalSerializer serializer;

        [SetUp]
        public void Init()
        {
            serializer = new HalSerializer();
            Fixture = GetFixture();
        }

        public void TestRoundTripJson(string json)
        {
            var expected = JObject.Parse(json).ToString();

            var builder = serializer.DeserializeToNewBuilder(expected, () => new RepresentorBuilder());

            var result = serializer.Serialize(builder.ToRepresentor());

            Assert.AreEqual(expected, JObject.Parse(result).ToString());
        }

        private const string SelfLinkOnly = @"{
            '_links': {
                'self': { 'href': '/example_resource' }
            }
        }";

        [Test]
        public void SelfLinkOnly_RoundTrip()
        {
            TestRoundTripJson(SelfLinkOnly);
        }

        private const string MultipleLinksSameRelation = @"
        {
            '_links': {
              'items': [{
                  'href': '/first_item'
              },{
                  'href': '/second_item'
              }]
            }
        }";

        [Test]
        public void MultipleLinksSameRelation_RoundTrip()
        {
            TestRoundTripJson(MultipleLinksSameRelation);
        }

        private const string SimpleLinksAndAttributes = @"{
        '_links': {
            'self': { 'href': '/orders' },
            'next': { 'href': '/orders?page=2' },
            'ea:find': {
                'href': '/orders{?id}'
            },
            'ea:admin': [{
                'href': '/admins/2',
                'title': 'Fred'
            }, {
                'href': '/admins/5',
                'title': 'Kate'
            }]
        },
        'currentlyProcessing': 14,
        'shippedToday': 20,
        }";

        [Test]
        public void SimpleLinksAndAttributes_RoundTrip()
        {
            TestRoundTripJson(SimpleLinksAndAttributes);
        }


        // From "Resources" here: https://phlyrestfully.readthedocs.org/en/latest/halprimer.html
        private const string ComplexEmbeddedResources = @"
        {
            '_links': {
                'self': {
                    'href': 'http://example.org/api/user/matthew'
                }
            },
            'id': 'matthew',
            'name': 'Matthew Weier O\'Phinney',
            '_embedded': {
                'contacts': [
                    {
                        '_links': {
                            'self': {
                                'href': 'http://example.org/api/user/mac_nibblet'
                            }
                        },
                        'id': 'mac_nibblet',
                        'name': 'Antoine Hedgecock'
                    },
                    {
                        '_links': {
                            'self': {
                                'href': 'http://example.org/api/user/spiffyjr'
                            }
                        },
                        'id': 'spiffyjr',
                        'name': 'Kyle Spraggs'
                    }
                ],
                'website': {
                    '_links': {
                        'self': {
                            'href': 'http://example.org/api/locations/mwop'
                        }
                    },
                    'id': 'mwop',
                    'url': 'http://www.mwop.net'
                },
            }
        }
        ";

        [Test]
        public void ComplexEmbeddedResources_RoundTrip()
        {
            TestRoundTripJson(ComplexEmbeddedResources);
        }

    }
}
