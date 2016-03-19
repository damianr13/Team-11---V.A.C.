int motor1a = 3;
int motor1b = 5;
int motor2a = 6;
int motor2b = 9;

char c;

void setup() {
  pinMode(motor1a, OUTPUT);
  pinMode(motor1b, OUTPUT);
  pinMode(motor2a, OUTPUT);
  pinMode(motor2b, OUTPUT);
  Serial.begin(9600);
  c = 'o';
}
void inainte()
{
  digitalWrite(motor1b,HIGH);
  digitalWrite(motor2b,HIGH);
  digitalWrite(motor1a,LOW);
  digitalWrite(motor2a,LOW);  
}
void inapoi()
{
  digitalWrite(motor1a,HIGH);
  digitalWrite(motor2a,HIGH);
  digitalWrite(motor1b,LOW);
  digitalWrite(motor2b,LOW);  
}
void stanga()
{
  digitalWrite(motor1b,LOW);
  digitalWrite(motor2b,HIGH);
  digitalWrite(motor1a,HIGH);
  digitalWrite(motor2a,LOW);
}
void dreapta()
{
  digitalWrite(motor1b,HIGH);
  digitalWrite(motor2b,LOW);
  digitalWrite(motor1a,LOW);
  digitalWrite(motor2a,HIGH);
}
void opreste()
{
  digitalWrite(motor1b,LOW);
  digitalWrite(motor2b,LOW);
  digitalWrite(motor1a,LOW);
  digitalWrite(motor2a,LOW);
}

void loop() {
  if(Serial.available() > 0){ 
    c=Serial.read();
  }
  if(c=='w') { inainte(); }
  if(c=='s') { inapoi(); }
  if(c=='d')
  {
    dreapta();
    delay(375);
    c='o';
  }
  if(c=='a')
  {
    stanga();
    delay(375);
    c='o';
  }
  if(c=='o'){ opreste();}
  if(c=='q')
  {
    stanga();
    delay(170);
    c='o';
   }
  if(c=='e')
  {
    dreapta();
    delay(170);
    c='o';
  }
  }

