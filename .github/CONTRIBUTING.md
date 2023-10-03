# How to contribute

Community contributions are essential part of any open source project. The community has access to a large number of unique configurations which would be extremely difficult for the core maintainers to reproduce. We want to keep it as easy as possible to contribute changes that get things working in your environment. There are a few guidelines that we need contributors to follow so that we can have a chance of keeping on top of things.

Make sure you have a [GitHub account](https://github.com/signup/free) to start with.

## Level 1 contributions: easy ways to contribute
There are several ways to contribute, depending on your background, level of expertise and willingness to spend some time. Everybody should be able to contribute on a minimal level:
* Create or document issues
* Create or improve documentation
* Be a tester

### Create issues
Anybody with a GutHub account can create new issues. Please verify that a simlar issue has not been created before. Clearly describe the issue you encounter.

### Create or improve documentation
Create an issue with suggestions for document improvements. If you have an idea about the new documenation, add that text (prefereably in markdown format) in the issue as proposed new or improved documentation.

### Be a tester
Download an install version of the extension and test it in your own environment. If you are reviewing issues, try to reproduce issues that have been created and enhance documentation of the issue to reproduce any bugs. 

If there is a new version of an extension, be one of the first to test that new version on a fresh install and as an upgrade of a previous version. Report your findings as issues.

# Level 2 contributions: Provide improved or new code 
If you are commited as community member and have programming skills, there is more that you can do:
* Create code fixes
* Create new functionality

We asume that you have some basic Git(Hub) knowledge and know how to work with fork, clone, create issue, commit, push, pull requests

## Get started with code contributions

### Fork the repository on GitHub
If this is your first time working with the repository, you will need to fork the repository to get your system configured for local development.

If you are new to Dnn Development, you can start by watching this video playlist which explains how to setup the required tools, your development environment and submit pull requests for modules, the Platform and the Persona Bar.
[How to make a pull requests playlist](https://www.youtube.com/playlist?list=PLIx1M8IdVvqZ0bnODGqJyxvONNPj5BzMP)

* Click fork on the project. You will get a fork of the repository in your own GitHub account
* Clone your fork locally with `git clone
* Add the *upstream connection* to the original repository, so you can rebase and update your fork with `git remote add upstream` to the base (the original) repository
* To update your fork to the latest, you can then run `git fetch upstream` followed by `git push`
  
## Making Changes
* Create a topic branch from where you want to base your work. This is usually a branch linked to the *issue #* your are trying to solve
* Make commits of logical units
* When ready to publish your changes, you can with `git push -u origin my_contribution`
* Make sure your pull request description tags the GitHub issue ID, so it is clear what issue you have fixed
* Make sure your commit messages are in the proper format

## Submitting changes
* Push your changes to a topic branch in your fork of the repository
* Submit a pull request to the original (upstream( repository 
* The committers will handle updating the associated issue in the DNN Tracker to ensure it gets the necessary code review and QA

## Acceptance of your changes
* We have a group of fellow developers that review pull requests submitted by developers like yourself
* If your changes look good, then changes are merged to an appropriate release
* You should get an email notification as we complete processing of your pull request

# Level 3 contributions: Become a repository custodian
If you are really commited, want to work with highly commited team members (like yourself?) and want to contibute by taking care of one or more repositories, you might want to become a *repository custodian*. 

Contact one of the team members of the organisation and discuss the opportunities.
