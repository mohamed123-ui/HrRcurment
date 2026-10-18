import { Pipe, PipeTransform, inject } from '@angular/core';
import { LanguageService } from '../../core/services/language';

const translations: Record<string, { ar: string; en: string }> = {
  brand: { ar: 'سمارت ريكروتمنت', en: 'SmartRecruitment' },
  home: { ar: 'الرئيسية', en: 'Home' },
  careers: { ar: 'الوظائف', en: 'Careers' },
  candidateProfile: { ar: 'ملفي المهني', en: 'My profile' },
  hrDashboard: { ar: 'لوحة HR', en: 'HR dashboard' },
  login: { ar: 'تسجيل الدخول', en: 'Sign in' },
  register: { ar: 'إنشاء حساب', en: 'Create account' },
  logout: { ar: 'تسجيل الخروج', en: 'Sign out' },
  language: { ar: 'English', en: 'العربية' },
  exploreJobs: { ar: 'استكشف الوظائف', en: 'Explore jobs' },
  welcomeBack: { ar: 'مرحبًا بعودتك', en: 'Welcome back' },
  signInTitle: { ar: 'سجّل الدخول', en: 'Sign in' },
  signInLead: {
    ar: 'أدخل بياناتك للوصول إلى حسابك.',
    en: 'Enter your details to access your account.',
  },
  email: { ar: 'البريد الإلكتروني', en: 'Email address' },
  password: { ar: 'كلمة المرور', en: 'Password' },
  rememberMe: { ar: 'تذكرني', en: 'Remember me' },
  forgotPassword: { ar: 'هل نسيت كلمة المرور؟', en: 'Forgot password?' },
  signInAction: { ar: 'دخول إلى الحساب', en: 'Sign in to your account' },
  noAccount: { ar: 'ليس لديك حساب؟', en: "Don't have an account?" },
  createNewAccount: { ar: 'أنشئ حسابًا جديدًا', en: 'Create a new account' },
  createAccountTitle: { ar: 'أنشئ حسابك', en: 'Create your account' },
  createAccountLead: {
    ar: 'ابدأ ملفك المهني وقدّم على أول فرصة.',
    en: 'Build your profile and apply to your first opportunity.',
  },
  fullName: { ar: 'الاسم بالكامل', en: 'Full name' },
  createAction: { ar: 'إنشاء الحساب', en: 'Create account' },
  alreadyAccount: { ar: 'لديك حساب بالفعل؟', en: 'Already have an account?' },
  jobsIntro: { ar: 'فرصتك التالية', en: 'Your next opportunity' },
  jobsTitle: { ar: 'ابحث عن مكانك', en: 'Find your place' },
  jobsSubtitle: { ar: 'في الفريق.', en: 'on the team.' },
  jobsLead: {
    ar: 'وظائف حقيقية، فرق طموحة، وتأثير يبدأ من أول يوم.',
    en: 'Real roles, ambitious teams, and impact from day one.',
  },
  searchJobs: { ar: 'ابحث بالمسمى أو المهارة', en: 'Search by title or skill' },
  allDepartments: { ar: 'كل الأقسام', en: 'All departments' },
  allLocations: { ar: 'كل المواقع', en: 'All locations' },
  jobDetails: { ar: 'تفاصيل الوظيفة', en: 'Job details' },
  applyJob: { ar: 'قدّم على الوظيفة', en: 'Apply for this role' },
  dashboardArea: { ar: 'مساحة الموارد البشرية', en: 'HR workspace' },
  dashboardTitle: { ar: 'صباح الخير', en: 'Good morning' },
  applicants: { ar: 'إجمالي المتقدمين', en: 'Total applicants' },
  activeJobs: { ar: 'الوظائف النشطة', en: 'Active jobs' },
  shortlisted: { ar: 'مرشحون مختصرون', en: 'Shortlisted candidates' },
  upcomingInterviews: { ar: 'مقابلات قادمة', en: 'Upcoming interviews' },
  candidateData: { ar: 'بياناتك', en: 'Your details' },
  completeProfile: { ar: 'أكمل ملفك المهني', en: 'Complete your profile' },
  applications: { ar: 'طلبات التوظيف', en: 'Applications' },
  saveProfile: { ar: 'حفظ الملف', en: 'Save profile' },
  editProfile: { ar: 'تعديل الملف', en: 'Edit profile' },
  cv: { ar: 'السيرة الذاتية', en: 'Resume' },
  uploadCv: { ar: 'رفع السيرة الذاتية', en: 'Upload resume' },
};

@Pipe({ name: 'translate', standalone: true, pure: false })
export class TranslatePipe implements PipeTransform {
  private readonly language = inject(LanguageService);

  transform(key: string): string {
    return translations[key]?.[this.language.language()] ?? key;
  }
}
