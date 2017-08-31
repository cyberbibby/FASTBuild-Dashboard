using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using FastBuild.Dashboard.Services;
using FastBuild.Dashboard.Services.Build;
using FastBuild.Dashboard.ViewModels.Build;

namespace FastBuild.Dashboard.Views.Build
{
	partial class BuildJobsView
	{
		// recoreds active (visible) jobs
		private readonly HashSet<BuildJobViewModel> _activeJobs
			= new HashSet<BuildJobViewModel>();

		protected override void Clear()
		{
			_activeJobs.Clear();
			_visibleCores.Clear();
			_coreTopMap.Clear();

			base.Clear();
		}

		private void TryAddJob(BuildJobViewModel job)
		{
			_activeJobs.Add(job);
		}

		private void UpdateJobs()
		{
			this.UpdateTimeFrame();

			var jobs = new HashSet<BuildJobViewModel>(this.JobManager.EnumerateJobs(this.StartTimeOffset, this.EndTimeOffset, _visibleCores));

			// remove job that are no longer existed in current time frame
			var jobsToRemove = _activeJobs.Where(job => !jobs.Contains(job)).ToList();

			foreach (var job in jobsToRemove)
			{
				_activeJobs.Remove(job);
			}

			// create view for jobs which are new to current time frame
			foreach (var job in jobs)
			{
				this.TryAddJob(job);
			}

			// will be filled in OnRender
			_jobBounds.Clear();

			this.InvalidateVisual();
		}

		private void UpdateTimeFrame()
		{
			this.StartTimeOffset = this.ViewportService.ViewStartTimeOffsetSeconds
			                       +
			                       (_headerViewWidth - 8) /
			                       this.ViewportService
				                       .Scaling; // minus 8px to make the jobs looks like being covered under the header panel

			this.EndTimeOffset = this.ViewportService.ViewEndTimeOffsetSeconds;
			_wasNowInTimeFrame = this.EndTimeOffset >= this.CurrentTimeOffset && this.StartTimeOffset <= this.CurrentTimeOffset;
		}
	}
}
