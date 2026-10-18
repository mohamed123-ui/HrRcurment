export interface AuthUser {
  userId: number;
  name: string;
  email: string;
  role: string;
  accessToken: string;
  refreshToken: string;
}

export interface CreateHrPayload {
  Name: string;
  Email: string;
  Password: string;
}

export interface Job {
  id: number;
  hrid: number;
  title: string;
  description: string;
  requiredSkills: string;
  experienceRequired: number;
  createdAt: string;
  isActive?: boolean;
}

export interface JobPayload {
  title: string;
  description: string;
  requiredSkills: string;
  experienceRequired: number;
}

export interface CandidateProfile {
  candidateId: number;
  name: string;
  email: string;
  age: number;
  educationLevel: string;
  universityTier: string;
  cgpa: number;
  internships: number;
  projects: number;
  programmingLanguages: number;
  certifications: number;
  experienceYears: number;
  hackathons: number;
  researchPapers: number;
  skillsScore: number;
  hired: boolean;
  softSkillsScore: number;
  resumeLengthWords: number;
  companyType: string;
  skills?: string;
  bio?: string;
}

export interface UpsertCandidateProfileDto {
  age: number;
  educationLevel: string;
  universityTier: string;
  cgpa: number;
  internships: number;
  projects: number;
  programmingLanguages: number;
  certifications: number;
  experienceYears: number;
  hackathons: number;
  skillsScore: number;
  companyType: string;
  skills?: string;
  bio?: string;
}

export interface Application {
  id: number;
  jobTitle: string;
  appliedAt: string;
  status: string;
  matchScore: number;
}

export interface Applicant {
  applicationId: number;
  jobId: number;
  jobTitle: string;
  candidateId: number;
  candidateName: string;
  email: string;
  phone?: string;
  appliedAt: string;
  status: string;
  matchScore?: number;
  skills: string[];
  yearsOfExperience: number;
  educationLevel: string;
  hasCv: boolean;
}

export interface ApplicantDetails extends Applicant {
  summary?: string;
  matchedSkills: string[];
  missingSkills: string[];
  mlFeedback?: string;
  statusHistory: StatusHistoryItem[];
  interview?: ApplicantInterview;
}

export interface StatusHistoryItem {
  status: string;
  changedAt: string;
  changedBy: string;
  note?: string;
}

export interface ApplicantInterview {
  id: number;
  scheduledAt: string;
  type: string;
  status: string;
}

export interface ShortlistedCandidate {
  applicationId: number;
  candidateName: string;
  jobTitle: string;
}

export interface Interview {
  id: number;
  applicationId: number;
  candidateName: string;
  jobId: number;
  jobTitle: string;
  type: string;
  scheduledAt: string;
  location?: string;
  status: string;
  result?: string;
  notes?: string;
}

export interface ScheduleInterviewPayload {
  applicationId: number;
  type: string;
  scheduledAt: string;
  location?: string;
}

export interface DashboardStats {
  activeJobs: number;
  totalApplicants: number;
  shortlistedCount: number;
  upcomingInterviewsCount: number;
  acceptedCount: number;
  rejectedCount: number;
  upcomingInterviews: UpcomingInterview[];
  applicantsByStatus: { status: string; count: number }[];
}

export interface UpcomingInterview {
  id: number;
  candidateName: string;
  jobTitle: string;
  type: string;
  scheduledAt: string;
  location?: string;
}

export type ApiEnvelope<T> = {
  isSuccess?: boolean;
  value?: T;
  errors?: string[];
};
