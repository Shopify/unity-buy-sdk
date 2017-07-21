#!/bin/bash
set -eo pipefail

if [ "$BUILDKITE" != "true" ] ; then
  echo "This should run on Buildkite!"
  exit 1
fi

function deactivate_license {
  echo "--- Deactivate Unity license"
  ./scripts/deactivate_license.sh
}

echo "--- Dependencies"
source ./scripts/prepare_ci.sh

echo "--- Build"
./scripts/build.sh

echo "--- Decrypting secrets...."
eval "$(rbenv init -)"
source <(ruby ./scripts/decrypt_secrets.rb)

echo "--- DEBUG env:"
env

echo "--- Activate Unity license"
./scripts/activate_license.sh

trap deactivate_license SIGHUP SIGINT SIGTERM EXIT

echo "+++ Test Unity"
./scripts/test_unity.sh

echo "+++ Test iOS"
./scripts/test_iOS.sh
