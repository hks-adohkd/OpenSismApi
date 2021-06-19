using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using X.PagedList;
//using X.PagedList;

namespace DBContext.Mapping
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public MappingProfile()
        {
            CreateMap<ApplicationUserViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, ApplicationUserViewModel>();

            CreateMap<CityViewModel, City>();
            CreateMap<City, CityViewModel>();
            CreateMap<IPagedList<City>, IPagedList<CityViewModel>>().ConvertUsing<PagedListConverter<City, CityViewModel>>();
            CreateMap<IPagedList<CityViewModel>, IPagedList<City>>().ConvertUsing<PagedListConverter<CityViewModel, City>>();

            CreateMap<ContentViewModel, Content>();
            CreateMap<Content, ContentViewModel>();
            CreateMap<IPagedList<ContentViewModel>, IPagedList<Content>>().ConvertUsing<PagedListConverter<ContentViewModel, Content>>();
            CreateMap<IPagedList<Content>, IPagedList<ContentViewModel>>().ConvertUsing<PagedListConverter<Content, ContentViewModel>>();

            CreateMap<AppTaskViewModel, AppTask>();
            CreateMap<AppTask, AppTaskViewModel>();
            CreateMap<IPagedList<AppTaskViewModel>, IPagedList<AppTask>>().ConvertUsing<PagedListConverter<AppTaskViewModel, AppTask>>();
            CreateMap<IPagedList<AppTask>, IPagedList<AppTaskViewModel>>().ConvertUsing<PagedListConverter<AppTask, AppTaskViewModel>>();

            CreateMap<ConditionViewModel, Condition>();
            CreateMap<Condition, ConditionViewModel>();
            CreateMap<IPagedList<ConditionViewModel>, IPagedList<Condition>>().ConvertUsing<PagedListConverter<ConditionViewModel, Condition>>();
            CreateMap<IPagedList<Condition>, IPagedList<ConditionViewModel>>().ConvertUsing<PagedListConverter<Condition, ConditionViewModel>>();

            CreateMap<ContactViewModel, Contact>();
            CreateMap<Contact, ContactViewModel>();
            CreateMap<IPagedList<ContactViewModel>, IPagedList<Contact>>().ConvertUsing<PagedListConverter<ContactViewModel, Contact>>();
            CreateMap<IPagedList<Contact>, IPagedList<ContactViewModel>>().ConvertUsing<PagedListConverter<Contact, ContactViewModel>>();

            CreateMap<ContactUsViewModel, ContactUs>();
            CreateMap<ContactUs, ContactUsViewModel>();
            CreateMap<IPagedList<ContactUsViewModel>, IPagedList<ContactUs>>().ConvertUsing<PagedListConverter<ContactUsViewModel, ContactUs>>();
            CreateMap<IPagedList<ContactUs>, IPagedList<ContactUsViewModel>>().ConvertUsing<PagedListConverter<ContactUs, ContactUsViewModel>>();

            CreateMap<MailViewModel, Mail>();
            CreateMap<Mail, MailViewModel>();
            CreateMap<IPagedList<MailViewModel>, IPagedList<Mail>>().ConvertUsing<PagedListConverter<MailViewModel, Mail>>();
            CreateMap<IPagedList<Mail>, IPagedList<MailViewModel>>().ConvertUsing<PagedListConverter<Mail, MailViewModel>>();

            CreateMap<CustomerViewModel, Customer>();
            CreateMap<Customer, CustomerViewModel>();
            CreateMap<IPagedList<CustomerViewModel>, IPagedList<Customer>>().ConvertUsing<PagedListConverter<CustomerViewModel, Customer>>();
            CreateMap<IPagedList<Customer>, IPagedList<CustomerViewModel>>().ConvertUsing<PagedListConverter<Customer, CustomerViewModel>>();

            CreateMap<CustomerMessageViewModel, CustomerMessage>();
            CreateMap<CustomerMessage, CustomerMessageViewModel>();
            CreateMap<IPagedList<CustomerMessageViewModel>, IPagedList<CustomerMessage>>().ConvertUsing<PagedListConverter<CustomerMessageViewModel, CustomerMessage>>();
            CreateMap<IPagedList<CustomerMessage>, IPagedList<CustomerMessageViewModel>>().ConvertUsing<PagedListConverter<CustomerMessage, CustomerMessageViewModel>>();

            CreateMap<CustomerPrizeViewModel, CustomerPrize>();
            CreateMap<CustomerPrize, CustomerPrizeViewModel>();
            CreateMap<IPagedList<CustomerPrizeViewModel>, IPagedList<CustomerPrize>>().ConvertUsing<PagedListConverter<CustomerPrizeViewModel, CustomerPrize>>();
            CreateMap<IPagedList<CustomerPrize>, IPagedList<CustomerPrizeViewModel>>().ConvertUsing<PagedListConverter<CustomerPrize, CustomerPrizeViewModel>>();

            CreateMap<CustomerTaskViewModel, CustomerTask>();
            CreateMap<CustomerTask, CustomerTaskViewModel>();
            CreateMap<IPagedList<CustomerTaskViewModel>, IPagedList<CustomerTask>>().ConvertUsing<PagedListConverter<CustomerTaskViewModel, CustomerTask>>();
            CreateMap<IPagedList<CustomerTask>, IPagedList<CustomerTaskViewModel>>().ConvertUsing<PagedListConverter<CustomerTask, CustomerTaskViewModel>>();

            CreateMap<GroupViewModel, Group>();
            CreateMap<Group, GroupViewModel>();
            CreateMap<IPagedList<GroupViewModel>, IPagedList<Group>>().ConvertUsing<PagedListConverter<GroupViewModel, Group>>();
            CreateMap<IPagedList<Group>, IPagedList<GroupViewModel>>().ConvertUsing<PagedListConverter<Group, GroupViewModel>>();

            CreateMap<MessageViewModel, Message>();
            CreateMap<Message, MessageViewModel>();
            CreateMap<IPagedList<MessageViewModel>, IPagedList<Message>>().ConvertUsing<PagedListConverter<MessageViewModel, Message>>();
            CreateMap<IPagedList<Message>, IPagedList<MessageViewModel>>().ConvertUsing<PagedListConverter<Message, MessageViewModel>>();

            CreateMap<MobileAppVersionViewModel, MobileAppVersion>();
            CreateMap<MobileAppVersion, MobileAppVersionViewModel>();
            CreateMap<IPagedList<MobileAppVersionViewModel>, IPagedList<MobileAppVersion>>().ConvertUsing<PagedListConverter<MobileAppVersionViewModel, MobileAppVersion>>();
            CreateMap<IPagedList<MobileAppVersion>, IPagedList<MobileAppVersionViewModel>>().ConvertUsing<PagedListConverter<MobileAppVersion, MobileAppVersionViewModel>>();

            CreateMap<PrizeViewModel, Prize>();
            CreateMap<Prize, PrizeViewModel>();
            CreateMap<IPagedList<PrizeViewModel>, IPagedList<Prize>>().ConvertUsing<PagedListConverter<PrizeViewModel, Prize>>();
            CreateMap<IPagedList<Prize>, IPagedList<PrizeViewModel>>().ConvertUsing<PagedListConverter<Prize, PrizeViewModel>>();

            CreateMap<PrizeTypeViewModel, PrizeType>();
            CreateMap<PrizeType, PrizeTypeViewModel>();
            CreateMap<IPagedList<PrizeTypeViewModel>, IPagedList<PrizeType>>().ConvertUsing<PagedListConverter<PrizeTypeViewModel, PrizeType>>();
            CreateMap<IPagedList<PrizeType>, IPagedList<PrizeTypeViewModel>>().ConvertUsing<PagedListConverter<PrizeType, PrizeTypeViewModel>>();

            CreateMap<TaskTypeViewModel, TaskType>();
            CreateMap<TaskType, TaskTypeViewModel>();
            CreateMap<IPagedList<TaskTypeViewModel>, IPagedList<TaskType>>().ConvertUsing<PagedListConverter<TaskTypeViewModel, TaskType>>();
            CreateMap<IPagedList<TaskType>, IPagedList<TaskTypeViewModel>>().ConvertUsing<PagedListConverter<TaskType, TaskTypeViewModel>>();

            CreateMap<SportMatchViewModel, SportMatch>();
            CreateMap<SportMatch, SportMatchViewModel>();
            CreateMap<IPagedList<SportMatchViewModel>, IPagedList<SportMatch>>().ConvertUsing<PagedListConverter<SportMatchViewModel, SportMatch>>();
            CreateMap<IPagedList<SportMatch>, IPagedList<SportMatchViewModel>>().ConvertUsing<PagedListConverter<SportMatch, SportMatchViewModel>>();

            CreateMap<CustomerPredictionViewModel, CustomerPrediction>();
            CreateMap<CustomerPrediction, CustomerPredictionViewModel>();
            CreateMap<IPagedList<CustomerPredictionViewModel>, IPagedList<CustomerPrediction>>().ConvertUsing<PagedListConverter<CustomerPredictionViewModel, CustomerPrediction>>();
            CreateMap<IPagedList<CustomerPrediction>, IPagedList<CustomerPredictionViewModel>>().ConvertUsing<PagedListConverter<CustomerPrediction, CustomerPredictionViewModel>>();

            CreateMap<DailyBonusViewModel, DailyBonus>();
            CreateMap<DailyBonus, DailyBonusViewModel>();
            CreateMap<IPagedList<DailyBonusViewModel>, IPagedList<DailyBonus>>().ConvertUsing<PagedListConverter<DailyBonusViewModel, DailyBonus>>();
            CreateMap<IPagedList<DailyBonus>, IPagedList<DailyBonusViewModel>>().ConvertUsing<PagedListConverter<DailyBonus, DailyBonusViewModel>>();

            CreateMap<LuckyWheelViewModel, LuckyWheel>();
            CreateMap<LuckyWheel, LuckyWheelViewModel>();
            CreateMap<IPagedList<LuckyWheelViewModel>, IPagedList<LuckyWheel>>().ConvertUsing<PagedListConverter<LuckyWheelViewModel, LuckyWheel>>();
            CreateMap<IPagedList<LuckyWheel>, IPagedList<LuckyWheelViewModel>>().ConvertUsing<PagedListConverter<LuckyWheel, LuckyWheelViewModel>>();

            CreateMap<CustomerAnswerViewModel, CustomerAnswer>();
            CreateMap<CustomerAnswer, CustomerAnswerViewModel>();
            CreateMap<IPagedList<CustomerAnswerViewModel>, IPagedList<CustomerAnswer>>().ConvertUsing<PagedListConverter<CustomerAnswerViewModel, CustomerAnswer>>();
            CreateMap<IPagedList<CustomerAnswer>, IPagedList<CustomerAnswerViewModel>>().ConvertUsing<PagedListConverter<CustomerAnswer, CustomerAnswerViewModel>>();


            CreateMap<QuestionViewModel, Question>();
            CreateMap<Question, QuestionViewModel>();
            CreateMap<IPagedList<QuestionViewModel>, IPagedList<Question>>().ConvertUsing<PagedListConverter<QuestionViewModel, Question>>();
            CreateMap<IPagedList<Question>, IPagedList<QuestionViewModel>>().ConvertUsing<PagedListConverter<Question, QuestionViewModel>>();

            CreateMap<QuestionOptionViewModel, QuestionOption>();
            CreateMap<QuestionOption, QuestionOptionViewModel>();
            CreateMap<IPagedList<QuestionOptionViewModel>, IPagedList<QuestionOption>>().ConvertUsing<PagedListConverter<QuestionOptionViewModel, QuestionOption>>();
            CreateMap<IPagedList<QuestionOption>, IPagedList<QuestionOptionViewModel>>().ConvertUsing<PagedListConverter<QuestionOption, QuestionOptionViewModel>>();

            CreateMap<QuizViewModel, Quiz>();
            CreateMap<Quiz, QuizViewModel>();
            CreateMap<IPagedList<QuizViewModel>, IPagedList<Quiz>>().ConvertUsing<PagedListConverter<QuizViewModel, Quiz>>();
            CreateMap<IPagedList<Quiz>, IPagedList<QuizViewModel>>().ConvertUsing<PagedListConverter<Quiz, QuizViewModel>>();

            CreateMap<QuizOptionViewModel, QuizOption>();
            CreateMap<QuizOption, QuizOptionViewModel>();
            CreateMap<IPagedList<QuizOptionViewModel>, IPagedList<QuizOption>>().ConvertUsing<PagedListConverter<QuizOptionViewModel, QuizOption>>();
            CreateMap<IPagedList<QuizOption>, IPagedList<QuizOptionViewModel>>().ConvertUsing<PagedListConverter<QuizOption, QuizOptionViewModel>>();

            CreateMap<QuizIndexViewModel, QuizIndex>();
            CreateMap<QuizIndex, QuizIndexViewModel>();
            CreateMap<IPagedList<QuizIndexViewModel>, IPagedList<QuizIndex>>().ConvertUsing<PagedListConverter<QuizIndexViewModel, QuizIndex>>();
            CreateMap<IPagedList<QuizIndex>, IPagedList<QuizIndexViewModel>>().ConvertUsing<PagedListConverter<QuizIndex, QuizIndexViewModel>>();


            CreateMissingTypeMaps = true;


        }
    }
}
