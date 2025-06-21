#!/usr/bin/env python3
"""
git_stats.py

Scan the current Git repo and output:
  - commits.csv: one row per commit (with commit messages and branches)
  - author_summary.csv: aggregated stats per author
Also prints top contributors.

Automatically finds the Git root directory.
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
    # Find Git root directory
    git_root = find_git_root(path)
    print(f"Found Git repository at: {git_root}")

    # open repo
    repo = Repo(git_root)
    if repo.bare:
        print("Repository is bare")
        sys.exit(1)

    commits_data = []
    for commit in repo.iter_commits('--all'):
        stats = commit.stats.total
        is_merge = len(commit.parents) > 1
        # find branches containing this commit
        try:
            raw_branches = repo.git.branch('--all', '--contains', commit.hexsha).splitlines()
            # clean branch names
            branches = [b.strip().lstrip('* ').replace('remotes/', '') for b in raw_branches]
        except Exception:
            branches = []

        # determine primary branch (first in sorted list)
        primary_branch = sorted(branches)[0] if branches else None

        commits_data.append({
            'hexsha': commit.hexsha,
            'author_name': commit.author.name,
            'author_email': commit.author.email,
            'date': commit.committed_datetime.isoformat(),
            'message': commit.message.strip().replace('\n', ' '),
            'files_changed': stats['files'],
            'insertions': stats['insertions'],
            'deletions': stats['deletions'],
            'total_changes': stats['insertions'] + stats['deletions'],
            'is_merge': is_merge,
            'primary_branch': primary_branch
        })

    # Get current directory for output files
    output_dir = os.getcwd()

    # per-commit CSV
    df_commits = pd.DataFrame(commits_data)
    df_commits.sort_values('date', inplace=True)
    commits_path = os.path.join(output_dir, 'commits.csv')
    df_commits.to_csv(commits_path, index=False)
    print(f"Wrote {commits_path} ({len(df_commits)} commits)")

    # aggregate per-author
    agg = df_commits.groupby('author_name').agg(
        total_commits=('hexsha', 'count'),
        total_merges=('is_merge', 'sum'),
        insertions=('insertions', 'sum'),
        deletions=('deletions', 'sum'),
        total_changes=('total_changes', 'sum')
    ).reset_index()

    # who topped each category?
    top_commits = agg['total_commits'].idxmax()
    top_merges = agg['total_merges'].idxmax()
    top_changes = agg['total_changes'].idxmax()

    agg['most_commits'] = False
    agg.loc[top_commits, 'most_commits'] = True
    agg['most_merges'] = False
    agg.loc[top_merges, 'most_merges'] = True
    agg['most_changes'] = False
    agg.loc[top_changes, 'most_changes'] = True

    summary_path = os.path.join(output_dir, 'author_summary.csv')
    agg.to_csv(summary_path, index=False)
    print(f"Wrote {summary_path} ({len(agg)} authors)")

    # print out top stats
    print("\nTop contributors:")
    print(" • Most commits:    {} ({} commits)".format(
        agg.loc[top_commits, 'author_name'],
        agg.loc[top_commits, 'total_commits']
    ))
    print(" • Most merge commits: {} ({} merges)".format(
        agg.loc[top_merges, 'author_name'],
        agg.loc[top_merges, 'total_merges']
    ))
    print(" • Most changes:    {} ({} lines changed)".format(
        agg.loc[top_changes, 'author_name'],
        agg.loc[top_changes, 'total_changes']
    ))


if __name__ == '__main__':
    # Start from current directory or provided path
    repo_path = sys.argv[1] if len(sys.argv) > 1 else os.getcwd()
    scan_repo(repo_path)
