#!/usr/bin/env bash
echo 'Restart all services in supervisor'
supervisorctl restart all 
echo 'Operation was successful'