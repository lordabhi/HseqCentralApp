using AutoMapper;
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;
using HseqCentralApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.App_Start
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            //AutoMapper.Mapper.CreateMap<Book, BookViewModel>()
            //    .ForMember(dest => dest.Author,
            //               opts => opts.MapFrom(src => src.Author.Name));

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Car, CarCreateViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<CarCreateViewModel, Car>()
                    .ForMember(dest => dest.HseqRecordID, opts => opts.Ignore())
                    .ForMember(dest => dest.AlfrescoNoderef, opts => opts.Ignore())
                    .ForMember(dest => dest.Coordinator, opts => opts.Ignore())
                    .ForMember(dest => dest.LinkedRecordsID, opts => opts.Ignore())
                    .ForMember(dest => dest.LinkedRecords, opts => opts.Ignore())
                    .ForMember(dest => dest.HseqCaseFileID, opts => opts.Ignore())
                    .ForMember(dest => dest.HseqCaseFile, opts => opts.Ignore())
                    .ForMember(dest => dest.DateLastUpdated, opts => opts.Ignore())
                    .ForMember(dest => dest.LastUpdatedBy, opts => opts.Ignore())
                    .ForMember(dest => dest.Delegatables, opts => opts.Ignore())
                    .ForMember(dest => dest.Comments, opts => opts.Ignore())
                    .ForMember(dest => dest.status, opts => opts.Ignore());
            });

        }

    }
}