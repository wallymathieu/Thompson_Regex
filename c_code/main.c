/*
 * main.c
 *
 *      Author: oskgew
 */
#include "nfa_test.h"
#include <stdio.h>

int main(int argc, char* argv[])
{
  int retval=0;
  #define maybe_return(X) retval = X();if (retval!=0){return retval;}

  printf("nfa_test\n");
  maybe_return(nfa_test);

  return 0;
}
