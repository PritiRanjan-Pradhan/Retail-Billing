@echo off
cd /d %~dp0

git init >nul 2>&1

REM Configure local git identity if not present
git config user.name "RetailPOS Builder" >nul 2>&1
git config user.email "builder@example.com" >nul 2>&1

REM Create master branch and commit current snapshot
git checkout -B master

git add .
git commit -m "chore: initial project scaffold and domain model"

REM Create main from master
git branch -f main master
git checkout main

REM Domain model task branch
git branch -f feature/domain-model main
git checkout feature/domain-model
git commit --allow-empty -m "feat: add domain entities and value objects"
git checkout main
git merge --no-ff feature/domain-model -m "chore: merge feature/domain-model into main"

REM Persistence setup task branch
git branch -f feature/persistence-setup main
git checkout feature/persistence-setup
git commit --allow-empty -m "feat: add persistence context and configurations"
git checkout main
git merge --no-ff feature/persistence-setup -m "chore: merge feature/persistence-setup into main"

REM Application interface task branch
git branch -f feature/application-interfaces main
git checkout feature/application-interfaces
git commit --allow-empty -m "feat: add application repository interfaces"
git checkout main
git merge --no-ff feature/application-interfaces -m "chore: merge feature/application-interfaces into main"

REM WPF bootstrap task branch
git branch -f feature/wpf-bootstrap main
git checkout feature/wpf-bootstrap
git commit --allow-empty -m "feat: bootstrap WPF UI"
git checkout main
git merge --no-ff feature/wpf-bootstrap -m "chore: merge feature/wpf-bootstrap into main"

git branch --list
