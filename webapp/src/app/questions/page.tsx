import { getQuestions } from "@/lib/actions/question-actions";
import QuestionCard from "./QuestionCard";
import QuestionsHeader from "./Questionsheader";

export default async function QuestionsPage({ searchParams }: Readonly<{ searchParams?: Promise<{ tag?: string }> }>) {
    const params = await searchParams;
    const questions = await getQuestions(params?.tag);

    return (
        <>
            <QuestionsHeader total={questions.length} tag={params?.tag} />
            {questions.map(question => (
                <div key={question.id} className="py-4 not-last:border-b w-full flex">
                    <QuestionCard question={question} key={question.id} />
                </div>
            ))}
        </>
    )
}