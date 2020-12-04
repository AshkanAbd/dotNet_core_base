#!/usr/bin/env bash
cd "$(dirname -- "$(realpath -- "$0")")" || exit
cd ../../../../ || exit
echo 'Pull...'
if ! git pull >>/dev/null 2>&1; then
  echo "Can't pull, try git stash"
  git stash >>/dev/null 2>&1
  echo "Pull again..."
  if ! git pull >>/dev/null 2>&1; then
    echo "Can't pull, even after git stash."
    echo "Check git repo."
    exit
  fi
fi
echo 'Operation was successful'
exit