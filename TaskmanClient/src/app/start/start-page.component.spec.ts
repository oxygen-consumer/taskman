import { TestBed } from '@angular/core/testing';
import { StartPageComponent } from './start-page.component';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StartPageComponent],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(StartPageComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have the 'TaskmanClient' title`, () => {
    const fixture = TestBed.createComponent(StartPageComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('TaskmanClient');
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(StartPageComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, TaskmanClient');
  });
});
