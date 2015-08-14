using AutoMapper;
using HseqCentralApp.App_Start;
using HseqCentralApp.Models;
using HseqCentralApp.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HseqCentralApp.Site.Tests.AutoMapper
{
    [TestFixture]
    public class AutoMapperTest
    {
        [Test]
        public void TestAutoMapper()
        {
            AutoMapperConfig.RegisterMappings();
            //Mapper.CreateMap<Car, CarViewModel>();

            Mapper.AssertConfigurationIsValid();
        }

    }
}
