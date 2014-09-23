#!/bin/sh
declare -a commands=(0 0 0)
while getopts ":suba" option; do
    case $option in
        s) commands[0]=1 ;;
        u) commands[1]=1 ;;
        b) commands[2]=1 ;;
        a) commands=(1 1 1) ;;
    esac
done

[[ ${commands[0]} -eq 1 ]] && echo "setup"
[[ ${commands[1]} -eq 1 ]] && echo "update"
[[ ${commands[2]} -eq 1 ]] && echo "build"
