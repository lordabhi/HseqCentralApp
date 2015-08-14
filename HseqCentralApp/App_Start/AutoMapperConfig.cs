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
                //CAR
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

                cfg.CreateMap<Car, CarEditViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<CarEditViewModel, Car>()
                    .ForMember(dest => dest.Coordinator, opts => opts.Ignore())
                    .ForMember(dest => dest.LinkedRecords, opts => opts.Ignore())
                    .ForMember(dest => dest.HseqCaseFile, opts => opts.Ignore())
                    .ForMember(dest => dest.Delegatables, opts => opts.Ignore())
                    .ForMember(dest => dest.Comments, opts => opts.Ignore())
                    .ForMember(dest => dest.status, opts => opts.Ignore());

                //PAR
                cfg.CreateMap<Par, ParCreateViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<ParCreateViewModel, Par>()
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

                cfg.CreateMap<Par, ParEditViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<ParEditViewModel, Par>()
                    .ForMember(dest => dest.Coordinator, opts => opts.Ignore())
                    .ForMember(dest => dest.LinkedRecords, opts => opts.Ignore())
                    .ForMember(dest => dest.HseqCaseFile, opts => opts.Ignore())
                    .ForMember(dest => dest.Delegatables, opts => opts.Ignore())
                    .ForMember(dest => dest.Comments, opts => opts.Ignore())
                    .ForMember(dest => dest.status, opts => opts.Ignore());

                //FIS
                cfg.CreateMap<Fis, FisCreateViewModel>()
                                   .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<FisCreateViewModel, Fis>()
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
                    .ForMember(dest => dest.FisCode, opts => opts.Ignore());

                cfg.CreateMap<Fis, FisEditViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<FisEditViewModel, Fis>()
                    .ForMember(dest => dest.Coordinator, opts => opts.Ignore())
                    .ForMember(dest => dest.LinkedRecords, opts => opts.Ignore())
                    .ForMember(dest => dest.HseqCaseFile, opts => opts.Ignore())
                    .ForMember(dest => dest.Delegatables, opts => opts.Ignore())
                    .ForMember(dest => dest.Comments, opts => opts.Ignore())
                    .ForMember(dest => dest.FisCode, opts => opts.Ignore());

                //NCR
                cfg.CreateMap<Ncr, NcrCreateViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<NcrCreateViewModel, Ncr>()
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
                    .ForMember(dest => dest.DiscrepancyType, opts => opts.Ignore())
                    .ForMember(dest => dest.DispositionType, opts => opts.Ignore())
                    .ForMember(dest => dest.DetectedInArea, opts => opts.Ignore())
                    .ForMember(dest => dest.ResponsibleArea, opts => opts.Ignore());

                cfg.CreateMap<Ncr, NcrEditViewModel>()
                    .ForMember(dest => dest.Coordinators, opts => opts.UseValue(Utils.AppUsers()));

                cfg.CreateMap<NcrEditViewModel, Ncr>()
                    .ForMember(dest => dest.Coordinator, opts => opts.Ignore())
                    .ForMember(dest => dest.LinkedRecords, opts => opts.Ignore())
                    .ForMember(dest => dest.HseqCaseFile, opts => opts.Ignore())
                    .ForMember(dest => dest.Delegatables, opts => opts.Ignore())
                    .ForMember(dest => dest.Comments, opts => opts.Ignore())
                    .ForMember(dest => dest.DiscrepancyType, opts => opts.Ignore())
                    .ForMember(dest => dest.DispositionType, opts => opts.Ignore())
                    .ForMember(dest => dest.DetectedInArea, opts => opts.Ignore())
                    .ForMember(dest => dest.ResponsibleArea, opts => opts.Ignore());

            });


        }

    }
}