#!/usr/bin/ruby

require 'json'

def print_export(key, value)
  puts "export #{key}=#{value}"
end

def decrypt_ejson(path)
  json = JSON.parse(`ejson decrypt #{path}`)
  json['env'].each do |key, value|
    print_export(key, value)
  end
end

raise "EJSON_FILE not set!" unless ENV['EJSON_FILE']
decrypt_ejson ENV['EJSON_FILE']
