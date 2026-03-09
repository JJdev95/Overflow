import { render, screen } from '@testing-library/react';
import QuestionCard from './QuestionCard';
import { Question } from '@/lib/types';
import { describe, it, expect, vi } from 'vitest';

// Mock next/link
vi.mock('next/link', () => {
  return {
    default: ({ children, href }: { children: React.ReactNode; href: string }) => {
      return <a href={href}>{children}</a>;
    },
  };
});

describe('QuestionCard', () => {
  it('renders the question title and asker correctly', () => {
    const mockQuestion: Question = {
      id: '1',
      title: 'How to use React Testing Library?',
      content: '<p>I am trying to learn how to test React components.</p>',
      tagSlugs: ['react', 'testing'],
      askerId: 'user123',
      createdAt: '2023-10-27T10:00:00Z',
      updatedAt: '2023-10-27T10:00:00Z',
      viewCount: 150,
      votes: 42,
      answerCount: 3,
      hasAcceptedAnswer: true,
      answers: [],
      askerDisplayName: ''
    };

    render(<QuestionCard question={mockQuestion} />);

    // Check if title is rendered
    expect(screen.getByText('How to use React Testing Library?')).toBeInTheDocument();

    // Check if stats are rendered
    expect(screen.getByText('42 votes')).toBeInTheDocument();
    expect(screen.getByText('150 views')).toBeInTheDocument();

    // Check if the asker name is rendered
    expect(screen.getByText('user123')).toBeInTheDocument();
  });
});
