//#include <SharpIR.h>
int IRpin = 8;                                   

void setup() {
  Serial.begin(9600);                            
}

void loop() {
  float volts = analogRead(IRpin)*0.0048828125;   
  float distance = 65*pow(volts, -1.10);          
  Serial.println(distance);                       
  delay(100);                                     
  }
