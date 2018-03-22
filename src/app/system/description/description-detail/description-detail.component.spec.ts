import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DescriptionDetailComponent } from './description-detail.component';

describe('DescriptionDetailComponent', () => {
  let component: DescriptionDetailComponent;
  let fixture: ComponentFixture<DescriptionDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DescriptionDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DescriptionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });
});
