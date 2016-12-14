die() {
    echo $@;
    exit 1;
}

out() {
    echo $@;
}

start() {
    out "Starting: $@"
}

end() {
    out "Finished: $@"
}

PROJECT_ROOT=${PROJECT_ROOT:-$(pwd)}
SCRIPTS_ROOT="$PROJECT_ROOT/scripts"
