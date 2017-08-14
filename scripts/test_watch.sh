#!/bin/sh

nodemon --exec "scripts/build.sh github && scripts/test.sh" -e .cs,.erb,.rb --ignore Assets/Shopify
