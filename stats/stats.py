#!/usr/bin/env python3
"""
git_stats.py

Scan a Git repository and output detailed CSVs containing:
  - commits.csv: Commit metadata with parent/tree relationships
  - commit_files.csv: File-level changes per commit
  - branches.csv: Branch references (local/remote)
  - tags.csv: Tag metadata (lightweight/annotated)
  - author_summary.csv: Aggregated contributor statistics

Automatically finds Git root directory. Avoids storing file content.
"""

import os
import sys
import pandas as pd
from git import Repo, InvalidGitRepositoryError


def find_git_root(start_path='.'):
    """Traverse upwards to find the root of the Git repository."""
    path = os.path.abspath(start_path)
    while True:
        if os.path.exists(os.path.join(path, '.git')):
            return path
        parent = os.path.dirname(path)
        if parent == path:  # Reached filesystem root
            break
        path = parent
    raise InvalidGitRepositoryError(f"No Git repository found from {start_path}")


def scan_repo(path='.'):
    # Find Git root
    git_root = find_git_root(path)
    print(f"Found Git repository at: {git_root}")

    # Open repository
    repo = Repo(git_root)
    if repo.bare:
        print("Repository is bare")
        sys.exit(1)

    # Data collection containers
    commits_data = []
    commit_files_data = []
    branches_data = []
    tags_data = []

    # Collect branch information
    for ref in repo.references:
        ref_name = ref.name
        is_remote = ref_name.startswith('refs/remotes/')
        branch_name = ref_name.replace('refs/heads/', '').replace('refs/remotes/', '')

        # Skip HEAD and other non-branch refs
        if not any([ref_name.startswith('refs/heads/'), ref_name.startswith('refs/remotes/')]):
            continue

        try:
            upstream = ref.tracking_branch().name if ref.tracking_branch() else None
        except:
            upstream = None

        branches_data.append({
            'branch_name': branch_name,
            'is_remote': is_remote,
            'commit_sha': ref.commit.hexsha,
            'upstream': upstream
        })

    # Collect tag information
    for tag in repo.tags:
        if tag.tag:  # Annotated tag
            tagger_name = tag.tag.tagger.name if tag.tag.tagger else None
            tagger_email = tag.tag.tagger.email if tag.tag.tagger else None
            tag_date = tag.tag.tagged_datetime.isoformat() if tag.tag.tagged_datetime else None
            tag_message = tag.tag.message.strip() if tag.tag.message else None
            commit_sha = tag.tag.object.hexsha
            tag_type = 'annotated'
        else:  # Lightweight tag
            tagger_name = tagger_email = tag_date = tag_message = None
            commit_sha = tag.commit.hexsha
            tag_type = 'lightweight'

        tags_data.append({
            'tag_name': tag.name,
            'tag_type': tag_type,
            'commit_sha': commit_sha,
            'tagger_name': tagger_name,
            'tagger_email': tagger_email,
            'tag_date': tag_date,
            'tag_message': tag_message
        })

    # Process commits
    for commit in repo.iter_commits('--all'):
        # Collect branches containing this commit
        try:
            raw_branches = repo.git.branch('--all', '--contains', commit.hexsha).splitlines()
            branches = [b.strip().lstrip('* ').replace('remotes/', '') for b in raw_branches]
            primary_branch = sorted(branches)[0] if branches else None
        except Exception:
            branches = []
            primary_branch = None

        # Basic commit stats
        stats = commit.stats.total
        is_merge = len(commit.parents) > 1
        parent_shas = [p.hexsha for p in commit.parents]

        # Commit metadata
        commits_data.append({
            'hexsha': commit.hexsha,
            'author_name': commit.author.name,
            'author_email': commit.author.email,
            'author_date': commit.authored_datetime.isoformat(),
            'committer_name': commit.committer.name,
            'committer_email': commit.committer.email,
            'committer_date': commit.committed_datetime.isoformat(),
            'message': commit.message.strip(),
            'parent_shas': ';'.join(parent_shas),
            'tree_sha': commit.tree.hexsha,
            'files_changed': stats['files'],
            'insertions': stats['insertions'],
            'deletions': stats['deletions'],
            'total_changes': stats['insertions'] + stats['deletions'],
            'is_merge': is_merge,
            'primary_branch': primary_branch
        })

        # File-level changes - FIXED HERE
        file_stats = commit.stats.files
        base = commit.parents[0] if commit.parents else None
        try:
            diff = commit.diff(base, R=True, create_patch=False)
        except Exception:
            diff = []

        for entry in diff:
            file_path = entry.b_path or entry.a_path
            # Get file stats with proper dictionary access
            change_stats = file_stats.get(file_path, {'insertions': 0, 'deletions': 0})

            commit_files_data.append({
                'commit_sha': commit.hexsha,
                'file_path': file_path,
                'change_type': entry.change_type,
                'old_file_path': entry.a_path,
                'new_file_path': entry.b_path,
                'insertions': change_stats.get('insertions', 0),
                'deletions': change_stats.get('deletions', 0)
            })

    # Get current directory for output
    output_dir = os.getcwd()

    # Write CSVs
    dfs = {
        'commits': pd.DataFrame(commits_data),
        'commit_files': pd.DataFrame(commit_files_data),
        'branches': pd.DataFrame(branches_data),
        'tags': pd.DataFrame(tags_data)
    }

    for name, df in dfs.items():
        path = os.path.join(output_dir, f'{name}.csv')
        df.to_csv(path, index=False)
        print(f"Wrote {path} ({len(df)} rows)")

    # Create author summary
    if commits_data:
        df_commits = pd.DataFrame(commits_data)
        agg = df_commits.groupby('author_name').agg(
            total_commits=('hexsha', 'count'),
            total_merges=('is_merge', 'sum'),
            insertions=('insertions', 'sum'),
            deletions=('deletions', 'sum'),
            total_changes=('total_changes', 'sum')
        ).reset_index()

        # Add top contributor flags
        for col in ['total_commits', 'total_merges', 'total_changes']:
            agg[f'top_{col}'] = agg[col] == agg[col].max()

        summary_path = os.path.join(output_dir, 'author_summary.csv')
        agg.to_csv(summary_path, index=False)
        print(f"Wrote {summary_path} ({len(agg)} authors)")

        # Print top contributors
        print("\nTop contributors:")
        print(
            f" • Most commits:    {agg.loc[agg.total_commits.idxmax(), 'author_name']} ({agg.total_commits.max()} commits)")
        print(
            f" • Most merges:     {agg.loc[agg.total_merges.idxmax(), 'author_name']} ({agg.total_merges.max()} merges)")
        print(
            f" • Most changes:    {agg.loc[agg.total_changes.idxmax(), 'author_name']} ({agg.total_changes.max()} lines)")


if __name__ == '__main__':
    repo_path = sys.argv[1] if len(sys.argv) > 1 else os.getcwd()
    scan_repo(repo_path)