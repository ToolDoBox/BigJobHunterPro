import type { ApplicationsAnalysis } from '@/services/analytics';

interface JobDescriptionAnalysisProps {
  analysis: ApplicationsAnalysis | null;
  isLoading: boolean;
}

export default function JobDescriptionAnalysis({ analysis, isLoading }: JobDescriptionAnalysisProps) {
  if (isLoading) {
    return (
      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-base text-amber mb-4">JOB DESCRIPTION INSIGHTS</h2>
        <div className="flex justify-center py-8">
          <div className="animate-pulse text-gray-500">Analyzing applications...</div>
        </div>
      </div>
    );
  }

  if (!analysis || analysis.totalApplicationsAnalyzed === 0) {
    return null; // Don't show section if no applications
  }

  const hasRoleKeywords = analysis.roleKeywords.length > 0;
  const hasSkills = analysis.topSkills.length > 0;

  if (!hasRoleKeywords && !hasSkills) {
    return null; // Don't show empty section
  }

  return (
    <div className="metal-panel">
      <div className="metal-panel-screws" />
      <h2 className="font-arcade text-base text-amber mb-2">JOB DESCRIPTION INSIGHTS</h2>
      <p className="text-xs text-gray-500 mb-4">
        Based on {analysis.totalApplicationsAnalyzed} application{analysis.totalApplicationsAnalyzed !== 1 ? 's' : ''} analyzed
      </p>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Role Keywords */}
        {hasRoleKeywords && (
          <div>
            <h3 className="text-sm font-semibold text-blaze mb-3">
              Role Keywords
            </h3>
            <p className="text-xs text-gray-500 mb-3">
              Common terms in the positions you've applied for
            </p>
            <div className="space-y-2">
              {analysis.roleKeywords.map((kw, index) => (
                <div key={kw.keyword} className="flex items-center gap-3">
                  <span className="text-xs text-gray-500 w-5">{index + 1}.</span>
                  <div className="flex-1 flex items-center gap-2">
                    <span className="text-sm font-mono text-gray-200">{kw.keyword}</span>
                    <div className="flex-1 h-2 bg-gray-800 rounded-full overflow-hidden">
                      <div
                        className="h-full bg-blaze"
                        style={{ width: `${Math.min(kw.percentage, 100)}%` }}
                      />
                    </div>
                    <span className="text-xs text-gray-400 w-12 text-right">
                      {kw.percentage.toFixed(0)}%
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* In-Demand Skills */}
        {hasSkills && (
          <div>
            <h3 className="text-sm font-semibold text-blaze mb-3">
              In-Demand Skills
            </h3>
            <p className="text-xs text-gray-500 mb-3">
              Skills frequently appearing in job descriptions
            </p>
            <div className="space-y-2">
              {analysis.topSkills.map((skill, index) => (
                <div key={skill.skill} className="flex items-center gap-3">
                  <span className="text-xs text-gray-500 w-5">{index + 1}.</span>
                  <div className="flex-1 flex items-center gap-2">
                    <span className="text-sm font-mono text-gray-200">{skill.skill}</span>
                    <div className="flex-1 h-2 bg-gray-800 rounded-full overflow-hidden">
                      <div
                        className="h-full bg-amber"
                        style={{ width: `${Math.min(skill.percentage, 100)}%` }}
                      />
                    </div>
                    <span className="text-xs text-gray-400 w-16 text-right">
                      {skill.count} ({skill.percentage.toFixed(0)}%)
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      <p className="text-xs text-gray-500 mt-6 border-t border-gray-700 pt-4">
        Use these insights to align your resume with in-demand skills and identify potential gaps in your profile.
      </p>
    </div>
  );
}
